class ReferenceFieldManager {
    constructor() {
        this.cache = new Map();
        this.debounceTimers = new Map();
        this.activeRequests = new Map();
    }

    init() {
        this.setupEventListeners();
        console.log('ReferenceFieldManager inicializado');
    }

    setupEventListeners() {
        document.addEventListener('DOMContentLoaded', () => {
            this.initializeAllFields();
        });

        document.addEventListener('click', (e) => {
            if (!e.target.closest('.reference-field-container')) {
                this.hideAllDropdowns();
            }
        });
    }

    initializeAllFields() {
        const fields = document.querySelectorAll('.reference-search-input');
        fields.forEach(input => {
            if (!input.dataset.initialized) {
                this.initializeField(input);
                input.dataset.initialized = 'true';
            }
        });

        const clearBtns = document.querySelectorAll('.reference-clear-btn');
        clearBtns.forEach(btn => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => this.clearSelection(e));
                btn.dataset.initialized = 'true';
            }
        });

        const createBtns = document.querySelectorAll('.reference-create-btn');
        createBtns.forEach(btn => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => this.openCreateModal(e));
                btn.dataset.initialized = 'true';
            }
        });
    }

    initializeField(input) {
        input.addEventListener('input', (e) => this.handleSearch(e));
        input.addEventListener('focus', (e) => this.handleFocus(e));
        input.addEventListener('blur', (e) => this.handleBlur(e));
        input.addEventListener('keydown', (e) => this.handleKeyDown(e));

        try {
            const filterConfig = input.dataset.referenceFilters;
            if (!filterConfig || filterConfig === '{}') {
                return;
            }

            const config = JSON.parse(filterConfig);

            for (const [, filterInfo] of Object.entries(config)) {
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value;
                    const sourceHiddenInput = document.querySelector(`input[name="${sourceFieldName}"]`);

                    if (sourceHiddenInput && !sourceHiddenInput.dataset.listenerAttached) {
                        const sourceFieldDisplayName = this.getFieldDisplayName(sourceFieldName);

                        sourceHiddenInput.addEventListener('change', () => {
                            const targetField = input.dataset.targetField;
                            const targetHiddenInput = document.querySelector(`input[name="${targetField}"]`);

                            if (targetHiddenInput && targetHiddenInput.value && targetHiddenInput.value !== '0') {
                                input.value = '';
                                targetHiddenInput.value = '';
                                input.classList.remove('selected');
                                this.hideDropdown(input);
                            }

                            if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                                input.disabled = true;
                                input.placeholder = `Selecione ${sourceFieldDisplayName} primeiro`;
                            } else {
                                input.disabled = false;
                                input.placeholder = input.dataset.originalPlaceholder || 'Digite para pesquisar...';
                            }
                        });

                        sourceHiddenInput.dataset.listenerAttached = 'true';

                        if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                            input.disabled = true;
                            input.placeholder = `Selecione ${sourceFieldDisplayName} primeiro`;
                        }
                    }
                }
            }
        } catch (error) {
            console.error('Erro ao configurar filtros:', error);
        }
    }

    getFieldDisplayName(fieldName) {
        const field = document.querySelector(`input[name="${fieldName}"]`);
        if (field) {
            const label = document.querySelector(`label[for="${fieldName}"]`);
            if (label) {
                return label.textContent.replace('*', '').trim();
            }
        }
        return fieldName;
    }

    async handleSearch(event) {
        const input = event.target;
        const searchTerm = input.value.trim();
        const referenceType = input.dataset.referenceType;
        const targetField = input.dataset.targetField;

        if (searchTerm.length < 2) {
            this.hideDropdown(input);
            return;
        }

        if (this.debounceTimers.has(targetField)) {
            clearTimeout(this.debounceTimers.get(targetField));
        }

        this.debounceTimers.set(targetField, setTimeout(async () => {
            await this.performSearch(input, searchTerm, referenceType, targetField);
        }, 300));
    }

    async performSearch(input, searchTerm, referenceType, targetField) {
        try {
            const filters = this.getFiltersForField(input);
            const cacheKey = `${referenceType}:${searchTerm}:${JSON.stringify(filters)}`;

            let results;
            if (this.cache.has(cacheKey)) {
                results = this.cache.get(cacheKey);
            } else {
                if (this.activeRequests.has(targetField)) {
                    this.activeRequests.get(targetField).abort();
                }

                const controller = new AbortController();
                this.activeRequests.set(targetField, controller);

                const controllerName = this.getControllerName(referenceType);
                let url = `/${controllerName}/Search?term=${encodeURIComponent(searchTerm)}`;

                if (filters && Object.keys(filters).length > 0) {
                    for (const [key, value] of Object.entries(filters)) {
                        url += `&${key}=${encodeURIComponent(value)}`;
                    }
                }

                const response = await fetch(url, {
                    signal: controller.signal,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (!response.ok) {
                    throw new Error(`Erro na busca: ${response.status}`);
                }

                results = await response.json();
                this.cache.set(cacheKey, results);
                this.activeRequests.delete(targetField);
            }

            this.displayResults(input, results);

        } catch (error) {
            if (error.name !== 'AbortError') {
                console.error('Erro na busca:', error);
                this.showDropdownError(input, 'Erro ao buscar dados');
            }
        }
    }

    getFiltersForField(input) {
        try {
            const filterConfig = input.dataset.referenceFilters;
            if (!filterConfig || filterConfig === '{}') {
                return {};
            }

            const config = JSON.parse(filterConfig);
            const filters = {};

            for (const [filterField, filterInfo] of Object.entries(config)) {
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value;
                    const sourceInput = document.querySelector(`input[name="${sourceFieldName}"]`);

                    if (sourceInput && sourceInput.value && sourceInput.value !== '0') {
                        filters[filterField] = sourceInput.value;
                    }
                } else {
                    filters[filterField] = filterInfo.value;
                }
            }

            return filters;
        } catch (error) {
            console.error('Erro ao obter filtros:', error);
            return {};
        }
    }

    displayResults(input, results) {
        const dropdown = this.getDropdown(input);

        if (!results || results.length === 0) {
            dropdown.innerHTML = '<div class="reference-dropdown-item disabled">Nenhum resultado encontrado</div>';
            dropdown.style.display = 'block';
            return;
        }

        dropdown.innerHTML = results.map((item, index) => `
            <div class="reference-dropdown-item" data-id="${item.id}" data-text="${item.text}" data-index="${index}">
                <div><strong>${item.text}</strong></div>
                ${item.subtitle ? `<small class="text-muted">${item.subtitle}</small>` : ''}
            </div>
        `).join('');

        dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)').forEach(item => {
            item.addEventListener('click', () => this.selectItem(input, item));
        });

        dropdown.style.display = 'block';
    }

    showDropdownError(input, message) {
        const dropdown = this.getDropdown(input);
        dropdown.innerHTML = `<div class="reference-dropdown-item disabled text-danger">${message}</div>`;
        dropdown.style.display = 'block';
    }

    getDropdown(input) {
        const container = input.closest('.reference-field-container');
        let dropdown = container.querySelector('.reference-dropdown');

        if (!dropdown) {
            dropdown = document.createElement('div');
            dropdown.className = 'reference-dropdown';
            dropdown.style.display = 'none';
            container.appendChild(dropdown);
        }

        return dropdown;
    }

    selectItem(input, item) {
        const id = item.dataset.id;
        const text = item.dataset.text;
        const targetField = input.dataset.targetField;

        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);

        if (hiddenInput) {
            hiddenInput.value = id;
            input.value = text;
            input.classList.add('selected');

            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            input.dispatchEvent(new Event('change', { bubbles: true }));

            input.dispatchEvent(new CustomEvent('reference:selected', {
                detail: { id, text, targetField },
                bubbles: true
            }));
        }

        this.hideDropdown(input);
    }

    handleFocus(event) {
        const input = event.target;
        if (input.classList.contains('selected')) {
            input.select();
        }
    }

    handleBlur(event) {
        setTimeout(() => {
            this.hideDropdown(event.target);
        }, 200);
    }

    handleKeyDown(event) {
        const input = event.target;
        const dropdown = this.getDropdown(input);

        if (dropdown.style.display === 'none') return;

        const items = dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)');
        const activeItem = dropdown.querySelector('.reference-dropdown-item.active');
        let currentIndex = activeItem ? Array.from(items).indexOf(activeItem) : -1;

        switch (event.key) {
            case 'ArrowDown':
                event.preventDefault();
                currentIndex = Math.min(currentIndex + 1, items.length - 1);
                this.setActiveItem(items, currentIndex);
                break;

            case 'ArrowUp':
                event.preventDefault();
                currentIndex = Math.max(currentIndex - 1, 0);
                this.setActiveItem(items, currentIndex);
                break;

            case 'Enter':
                event.preventDefault();
                if (activeItem) {
                    this.selectItem(input, activeItem);
                }
                break;

            case 'Escape':
                this.hideDropdown(input);
                break;
        }
    }

    setActiveItem(items, index) {
        items.forEach((item, i) => {
            if (i === index) {
                item.classList.add('active');
                item.scrollIntoView({ block: 'nearest' });
            } else {
                item.classList.remove('active');
            }
        });
    }

    hideDropdown(input) {
        const dropdown = this.getDropdown(input);
        dropdown.style.display = 'none';
    }

    hideAllDropdowns() {
        document.querySelectorAll('.reference-dropdown').forEach(dropdown => {
            dropdown.style.display = 'none';
        });
    }

    clearSelection(event) {
        event.preventDefault();
        event.stopPropagation();

        const btn = event.target.closest('.reference-clear-btn');
        const container = btn.closest('.reference-field-container');
        const searchInput = container.querySelector('.reference-search-input');
        const targetField = searchInput.dataset.targetField;
        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);

        if (hiddenInput && searchInput) {
            searchInput.value = '';
            hiddenInput.value = '';
            searchInput.classList.remove('selected');

            this.hideDropdown(searchInput);

            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            searchInput.dispatchEvent(new Event('change', { bubbles: true }));

            searchInput.dispatchEvent(new CustomEvent('reference:cleared', {
                detail: { targetField },
                bubbles: true
            }));

            searchInput.focus();
        }
    }

    async openCreateModal(event) {
        event.preventDefault();
        event.stopPropagation();

        const btn = event.target.closest('.reference-create-btn');
        const referenceType = btn.dataset.referenceType;
        const targetField = btn.dataset.targetField;

        try {
            const controller = this.getControllerName(referenceType);
            const modal = this.createModal(controller, referenceType, targetField);
            document.body.appendChild(modal);

            await this.loadCreateContent(modal, controller);

            const bsModal = new bootstrap.Modal(modal, {
                backdrop: 'static',
                keyboard: true
            });
            bsModal.show();

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });

        } catch (error) {
            console.error('Erro ao abrir modal:', error);
            this.showToast('Erro ao abrir modal de criação', 'error');
        }
    }

    getControllerName(referenceType) {
        return referenceType + 's';
    }

    createModal(controller, referenceType, targetField) {
        const modalId = `createModal_${targetField}_${Date.now()}`;
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = modalId;
        modal.setAttribute('tabindex', '-1');
        modal.setAttribute('data-target-field', targetField);
        modal.setAttribute('data-controller', controller);

        modal.innerHTML = `
            <div class="modal-dialog modal-xl" style="max-width: 90%; width: 1400px;">
                <div class="modal-content" style="min-height: 85vh; max-height: 95vh; display: flex; flex-direction: column;">
                    <div class="modal-header bg-primary text-white" style="flex-shrink: 0;">
                        <h5 class="modal-title">
                            <i class="fas fa-plus me-2"></i>
                            Criar ${referenceType}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0" style="flex: 1; overflow: hidden; display: flex; flex-direction: column;">
                        <div class="d-flex justify-content-center align-items-center py-5" style="flex: 1;">
                            <div class="text-center">
                                <div class="spinner-border text-primary" role="status">
                                    <span class="visually-hidden">Carregando...</span>
                                </div>
                                <p class="mt-3 text-muted">Carregando formulário...</p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer" style="flex-shrink: 0; background: #f8f9fa; border-top: 1px solid #dee2e6;">
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>
                            Cancelar
                        </button>
                        <button type="button" class="btn btn-primary" id="modalSaveBtn" style="display: none;">
                            <i class="fas fa-save me-2"></i>
                            Salvar
                        </button>
                    </div>
                </div>
            </div>
        `;

        return modal;
    }

    async loadCreateContent(modal, controller) {
        try {
            const response = await fetch(`/${controller}/Create?modal=true`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao carregar formulário: ${response.status}`);
            }

            const html = await response.text();
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const saveBtn = modal.querySelector('#modalSaveBtn');
            if (saveBtn) {
                saveBtn.style.display = 'inline-flex';
            }

            this.setupModalForm(modal, controller);
            this.initializeFormElements(modalBody);

        } catch (error) {
            console.error('Erro ao carregar conteúdo:', error);
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = `
                <div class="alert alert-danger m-4">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    Erro ao carregar formulário. Por favor, tente novamente.
                </div>
            `;
        }
    }

    setupModalForm(modal, controller) {
        const form = modal.querySelector('form');
        if (!form) return;

        const saveBtn = modal.querySelector('#modalSaveBtn');

        if (saveBtn) {
            saveBtn.onclick = async (e) => {
                e.preventDefault();
                e.stopPropagation();

                if (!form.checkValidity()) {
                    form.classList.add('was-validated');
                    this.showToast('Por favor, preencha todos os campos obrigatórios', 'warning');
                    return;
                }

                const originalBtnContent = saveBtn.innerHTML;
                saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Salvando...';
                saveBtn.disabled = true;

                try {
                    const formData = new FormData(form);
                    const response = await fetch(form.action, {
                        method: 'POST',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        },
                        body: formData
                    });

                    if (response.ok) {
                        const contentType = response.headers.get('content-type');

                        if (contentType && contentType.includes('application/json')) {
                            const result = await response.json();

                            if (result.success) {
                                this.showToast(result.message || 'Registro criado com sucesso!', 'success');

                                const targetField = modal.dataset.targetField;
                                const hiddenInput = document.querySelector(`input[name="${targetField}"]`);
                                const searchInput = document.querySelector(`input.reference-search-input[data-target-field="${targetField}"]`);

                                if (hiddenInput && searchInput) {
                                    hiddenInput.value = result.id;
                                    searchInput.value = result.text || result.name;
                                    searchInput.classList.add('selected');

                                    hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
                                    searchInput.dispatchEvent(new Event('change', { bubbles: true }));

                                    const referenceType = searchInput.dataset.referenceType;
                                    if (referenceType && this.cache) {
                                        for (const [key] of this.cache) {
                                            if (key.startsWith(`${referenceType}:`)) {
                                                this.cache.delete(key);
                                            }
                                        }
                                    }
                                }

                                const bsModal = bootstrap.Modal.getInstance(modal);
                                if (bsModal) {
                                    bsModal.hide();
                                }

                            } else {
                                this.showValidationErrors(form, result.errors || {});
                            }
                        } else {
                            const html = await response.text();
                            const modalBody = modal.querySelector('.modal-body');
                            modalBody.innerHTML = html;
                            this.setupModalForm(modal, controller);
                            this.initializeFormElements(modalBody);
                        }
                    } else {
                        throw new Error(`Erro ${response.status}: ${response.statusText}`);
                    }

                } catch (error) {
                    console.error('Erro ao criar registro:', error);
                    this.showToast(`Erro ao criar registro: ${error.message}`, 'error');
                } finally {
                    if (saveBtn) {
                        saveBtn.innerHTML = originalBtnContent;
                        saveBtn.disabled = false;
                    }
                }
            };
        }
    }

    initializeFormElements(container) {
        if (typeof $.fn.mask !== 'undefined') {
            const $container = $(container);

            $container.find('.cpf-mask').mask('000.000.000-00');
            $container.find('.cnpj-mask').mask('00.000.000/0000-00');
            $container.find('.telefone-mask').mask('(00) 0000-00009');
            $container.find('.cep-mask').mask('00000-000');
            $container.find('.placa-mask').mask('AAA-0A00');
            $container.find('.renavam-mask').mask('00000000000');
            $container.find('.chassi-mask').mask('AAAAAAAAAAAAAAAAA');
            $container.find('.money-mask').mask('#.##0,00', { reverse: true });
        }

        const newRefFields = container.querySelectorAll('.reference-search-input');
        newRefFields.forEach(input => {
            if (!input.dataset.initialized) {
                this.initializeField(input);
                input.dataset.initialized = 'true';
            }
        });

        const newClearBtns = container.querySelectorAll('.reference-clear-btn');
        newClearBtns.forEach(btn => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => this.clearSelection(e));
                btn.dataset.initialized = 'true';
            }
        });

        const newCreateBtns = container.querySelectorAll('.reference-create-btn');
        newCreateBtns.forEach(btn => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => this.openCreateModal(e));
                btn.dataset.initialized = 'true';
            }
        });
    }

    showValidationErrors(form, errors) {
        form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
        form.querySelectorAll('.invalid-feedback').forEach(el => el.remove());

        for (const [field, messages] of Object.entries(errors)) {
            const input = form.querySelector(`[name="${field}"]`);
            if (input) {
                input.classList.add('is-invalid');
                const feedback = document.createElement('div');
                feedback.className = 'invalid-feedback';
                feedback.textContent = Array.isArray(messages) ? messages[0] : messages;
                input.parentElement.appendChild(feedback);
            }
        }

        this.showToast('Por favor, corrija os erros no formulário', 'error');
    }

    showToast(message, type = 'info') {
        if (typeof Toastify !== 'undefined') {
            Toastify({
                text: message,
                duration: 3000,
                gravity: 'top',
                position: 'right',
                backgroundColor: type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#17a2b8',
            }).showToast();
        } else {
            alert(message);
        }
    }
}

const referenceFieldManager = new ReferenceFieldManager();

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => referenceFieldManager.init());
} else {
    referenceFieldManager.init();
}

window.referenceFieldManager = referenceFieldManager;