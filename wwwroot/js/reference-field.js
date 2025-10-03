/*!
 * Reference Field Manager - Sistema de busca e criação para campos de referência
 * Versão 2.0 - Atualizada para API /api/Reference/Search com POST
 */

class ReferenceFieldManager {
    constructor() {
        this.debounceTimer = null;
        this.cache = new Map();
        this.loadingRequests = new Set();
        this.init();
    }

    init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                this.setupAll();
            });
        } else {
            this.setupAll();
        }
    }

    setupAll() {
        this.attachSearchHandlers();
        this.attachClearHandlers();
        this.attachCreateHandlers();
        this.setupDependencyWatchers();
    }

    attachSearchHandlers() {
        const searchInputs = document.querySelectorAll('.reference-search-input');
        searchInputs.forEach(input => {
            input.addEventListener('input', (e) => this.handleSearch(e));
            input.addEventListener('focus', (e) => this.handleFocus(e));
            input.addEventListener('blur', (e) => this.handleBlur(e));
            input.addEventListener('keydown', (e) => this.handleKeyDown(e));
        });
    }

    attachClearHandlers() {
        const clearBtns = document.querySelectorAll('.reference-clear-btn');
        clearBtns.forEach(btn => {
            btn.addEventListener('click', (e) => this.clearSelection(e));
        });
    }

    attachCreateHandlers() {
        const createBtns = document.querySelectorAll('.reference-create-btn');
        createBtns.forEach(btn => {
            btn.addEventListener('click', (e) => this.openCreateModal(e));
        });
    }

    async handleSearch(event) {
        const input = event.target;
        const searchTerm = input.value.trim();
        const referenceType = input.dataset.referenceType;
        const targetField = input.dataset.targetField;

        clearTimeout(this.debounceTimer);

        if (searchTerm.length < 2) {
            this.hideDropdown(input);
            return;
        }

        const cacheKey = `${referenceType}:${searchTerm.toLowerCase()}`;
        if (this.cache.has(cacheKey)) {
            this.showDropdown(input, this.cache.get(cacheKey));
            return;
        }

        this.debounceTimer = setTimeout(() => {
            this.performSearch(input, referenceType, searchTerm);
        }, 300);
    }

    async performSearch(input, referenceType, searchTerm) {
        const targetField = input.dataset.targetField;
        const requestKey = `${targetField}:${searchTerm}`;

        if (this.loadingRequests.has(requestKey)) {
            return;
        }

        this.loadingRequests.add(requestKey);
        this.showLoading(input);

        try {
            const filters = this.buildFilters(input);

            const response = await fetch('/api/Reference/Search', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    entityType: referenceType,
                    searchTerm: searchTerm,
                    pageSize: 10,
                    filters: filters
                })
            });

            if (response.ok) {
                const results = await response.json();

                const cacheKey = `${referenceType}:${searchTerm.toLowerCase()}`;
                this.cache.set(cacheKey, results);

                this.showDropdown(input, results);
            } else {
                console.error('Erro na busca:', response.status, response.statusText);
                this.showError(input, `Erro ao buscar dados (${response.status})`);
            }
        } catch (error) {
            console.error('Erro ao buscar referências:', error);
            this.showError(input, 'Erro de conexão. Tente novamente.');
        } finally {
            this.loadingRequests.delete(requestKey);
        }
    }

    showDropdown(input, results) {
        let dropdown = input.parentElement.parentElement.querySelector('.reference-dropdown');

        if (!dropdown) {
            dropdown = document.createElement('div');
            dropdown.className = 'reference-dropdown';
            input.parentElement.parentElement.appendChild(dropdown);
        }

        if (results.length === 0) {
            dropdown.innerHTML = '<div class="reference-dropdown-item disabled">Nenhum resultado encontrado</div>';
        } else {
            dropdown.innerHTML = results.map((item, index) => `
                <div class="reference-dropdown-item ${index === 0 ? 'active' : ''}" data-id="${item.value}" data-text="${item.text}">
                    ${item.text}
                    ${item.subtitle ? `<small class="text-muted d-block">${item.subtitle}</small>` : ''}
                </div>
            `).join('');

            dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)').forEach(item => {
                item.addEventListener('click', (e) => this.selectItem(e, input));
                item.addEventListener('mouseenter', () => {
                    dropdown.querySelectorAll('.reference-dropdown-item').forEach(i => i.classList.remove('active'));
                    item.classList.add('active');
                });
            });
        }

        dropdown.style.display = 'block';
    }

    showLoading(input) {
        let dropdown = input.parentElement.parentElement.querySelector('.reference-dropdown');

        if (!dropdown) {
            dropdown = document.createElement('div');
            dropdown.className = 'reference-dropdown';
            input.parentElement.parentElement.appendChild(dropdown);
        }

        dropdown.innerHTML = `
            <div class="reference-dropdown-item disabled">
                <div class="d-flex align-items-center gap-2">
                    <span class="spinner-border spinner-border-sm" role="status"></span>
                    <span>Buscando...</span>
                </div>
            </div>
        `;
        dropdown.style.display = 'block';
    }

    showError(input, message) {
        let dropdown = input.parentElement.parentElement.querySelector('.reference-dropdown');

        if (!dropdown) {
            dropdown = document.createElement('div');
            dropdown.className = 'reference-dropdown';
            input.parentElement.parentElement.appendChild(dropdown);
        }

        dropdown.innerHTML = `<div class="reference-dropdown-item disabled text-danger">${message}</div>`;
        dropdown.style.display = 'block';
    }

    hideDropdown(input) {
        const dropdown = input.parentElement.parentElement.querySelector('.reference-dropdown');
        if (dropdown) {
            dropdown.style.display = 'none';
        }
    }

    handleFocus(event) {
        const input = event.target;
        if (input.value.length >= 2) {
            const referenceType = input.dataset.referenceType;
            const searchTerm = input.value.trim();
            const cacheKey = `${referenceType}:${searchTerm.toLowerCase()}`;

            if (this.cache.has(cacheKey)) {
                this.showDropdown(input, this.cache.get(cacheKey));
            }
        }
    }

    handleBlur(event) {
        const input = event.target;
        setTimeout(() => {
            this.hideDropdown(input);
        }, 200);
    }

    handleKeyDown(event) {
        const input = event.target;
        const dropdown = input.parentElement.parentElement.querySelector('.reference-dropdown');

        if (!dropdown || dropdown.style.display === 'none') {
            return;
        }

        const items = Array.from(dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)'));
        if (items.length === 0) {
            return;
        }

        const currentActive = dropdown.querySelector('.reference-dropdown-item.active');
        let currentIndex = currentActive ? items.indexOf(currentActive) : -1;

        switch (event.key) {
            case 'ArrowDown':
                event.preventDefault();
                currentIndex = currentIndex < items.length - 1 ? currentIndex + 1 : 0;
                this.highlightItem(items, currentIndex);
                break;

            case 'ArrowUp':
                event.preventDefault();
                currentIndex = currentIndex > 0 ? currentIndex - 1 : items.length - 1;
                this.highlightItem(items, currentIndex);
                break;

            case 'Enter':
                event.preventDefault();
                if (currentActive) {
                    currentActive.click();
                }
                break;

            case 'Escape':
                event.preventDefault();
                this.hideDropdown(input);
                break;
        }
    }

    highlightItem(items, index) {
        items.forEach((item, i) => {
            if (i === index) {
                item.classList.add('active');
                item.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
            } else {
                item.classList.remove('active');
            }
        });
    }

    selectItem(event, input) {
        const item = event.target;
        const id = item.dataset.id;
        const text = item.dataset.text;
        const targetField = input.dataset.targetField;

        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);
        if (hiddenInput) {
            hiddenInput.value = id;
        }

        input.value = text;
        input.classList.add('selected');

        this.hideDropdown(input);

        hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
        input.dispatchEvent(new Event('change', { bubbles: true }));

        input.dispatchEvent(new CustomEvent('reference:selected', {
            detail: { id, text, targetField },
            bubbles: true
        }));
    }

    buildFilters(input) {
        const filters = {};

        try {
            const filterConfig = input.dataset.referenceFilters;
            if (!filterConfig || filterConfig === '{}') {
                return filters;
            }

            const config = JSON.parse(filterConfig);

            for (const [filterField, filterInfo] of Object.entries(config)) {
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value;
                    const sourceHiddenInput = document.querySelector(`input[name="${sourceFieldName}"]`);

                    if (sourceHiddenInput && sourceHiddenInput.value && sourceHiddenInput.value !== '0') {
                        filters[filterField] = sourceHiddenInput.value;
                    }
                } else {
                    filters[filterField] = filterInfo.value;
                }
            }
        } catch (error) {
            console.error('Erro ao processar filtros de referência:', error);
        }

        return filters;
    }

    setupDependencyWatchers() {
        const referenceInputs = document.querySelectorAll('.reference-search-input');

        referenceInputs.forEach(input => {
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

                        if (sourceHiddenInput) {
                            // Buscar o DisplayName do campo de origem
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
                                    input.placeholder = `Selecione o campo ${sourceFieldDisplayName} primeiro`;
                                } else {
                                    input.disabled = false;
                                    input.placeholder = input.dataset.originalPlaceholder || 'Digite para pesquisar...';
                                }
                            });

                            if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                                input.disabled = true;
                                input.dataset.originalPlaceholder = input.placeholder;
                                input.placeholder = `Selecione o campo ${sourceFieldDisplayName} primeiro`;
                            }
                        }
                    }
                }
            } catch (error) {
                console.error('Erro ao configurar observadores de dependência:', error);
            }
        });
    }

    getFieldDisplayName(fieldName) {
        // Buscar o label do campo no DOM
        const fieldContainer = document.querySelector(`[data-field-name="${fieldName}"]`);
        if (fieldContainer) {
            const label = fieldContainer.querySelector('.form-label, .form-label-modern, label');
            if (label) {
                // Remover asterisco de obrigatório e ícones
                let displayName = label.textContent.trim();
                displayName = displayName.replace(/\s*\*\s*$/, ''); // Remove asterisco no final
                displayName = displayName.replace(/^\s*\S+\s+/, ''); // Remove ícone no início (ex: " Marca")
                return displayName.trim();
            }
        }

        // Fallback: tentar encontrar pelo ID do input
        const input = document.querySelector(`#${fieldName}_search, input[name="${fieldName}"]`);
        if (input) {
            const container = input.closest('.form-group, .form-group-modern');
            if (container) {
                const label = container.querySelector('.form-label, .form-label-modern, label');
                if (label) {
                    let displayName = label.textContent.trim();
                    displayName = displayName.replace(/\s*\*\s*$/, '');
                    displayName = displayName.replace(/^\s*\S+\s+/, '');
                    return displayName.trim();
                }
            }
        }

        // Fallback final: usar o nome do campo formatado
        return this.formatFieldName(fieldName);
    }

    formatFieldName(fieldName) {
        // Converte "IdVeiculoMarca" para "Veiculo Marca"
        return fieldName
            .replace(/^Id/, '') // Remove "Id" do início
            .replace(/([A-Z])/g, ' $1') // Adiciona espaço antes de maiúsculas
            .trim();
    }

    clearSelection(event) {
        event.preventDefault();
        event.stopPropagation();

        const btn = event.target.closest('.reference-clear-btn');
        const container = btn.closest('.reference-field-container');
        const searchInput = container.querySelector('.reference-search-input');
        const hiddenInput = container.querySelector('input[type="hidden"]');

        if (confirm('Deseja limpar a seleção?')) {
            searchInput.value = '';
            hiddenInput.value = '';
            searchInput.classList.remove('selected');

            this.hideDropdown(searchInput);

            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            searchInput.dispatchEvent(new Event('change', { bubbles: true }));

            searchInput.dispatchEvent(new CustomEvent('reference:cleared', {
                detail: { targetField: searchInput.dataset.targetField },
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
        // Convenção: Nome da entidade + "s" = nome do controller
        // Exemplos: Cliente -> Clientes, Veiculo -> Veiculos, VeiculoMarca -> VeiculoMarcas
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
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-plus me-2"></i>
                            Criar ${referenceType}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0">
                        <div class="d-flex justify-content-center align-items-center py-5">
                            <div class="text-center">
                                <div class="spinner-border text-primary" role="status">
                                    <span class="visually-hidden">Carregando...</span>
                                </div>
                                <p class="mt-3 text-muted">Carregando formulário...</p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>
                            Cancelar
                        </button>
                        <button type="button" class="btn btn-success btn-save-modal" disabled>
                            <i class="fas fa-save me-2"></i>
                            Salvar
                            <span class="spinner-border spinner-border-sm ms-2 d-none" role="status">
                                <span class="visually-hidden">Salvando...</span>
                            </span>
                        </button>
                    </div>
                </div>
            </div>
        `;

        return modal;
    }

    async loadCreateContent(modal, controller) {
        try {
            const response = await fetch(`/${controller}/Create?ajax=true`, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Accept': 'text/html, application/xhtml+xml, text/plain, */*'
                }
            });

            if (response.ok) {
                const html = await response.text();
                const modalBody = modal.querySelector('.modal-body');

                modalBody.innerHTML = `<div class="container-fluid p-4">${html}</div>`;

                this.setupModalForm(modal, controller);
                this.initializeFormElements(modalBody);

                const saveBtn = modal.querySelector('.btn-save-modal');
                saveBtn.disabled = false;

            } else {
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }
        } catch (error) {
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = `
                <div class="container-fluid p-4">
                    <div class="alert alert-danger">
                        <i class="fas fa-exclamation-triangle me-2"></i>
                        <strong>Erro ao carregar formulário:</strong><br>
                        ${error.message}
                        <hr>
                        <button type="button" class="btn btn-outline-danger btn-sm" onclick="location.reload()">
                            <i class="fas fa-refresh me-1"></i> Recarregar Página
                        </button>
                    </div>
                </div>
            `;
        }
    }

    setupModalForm(modal, controller) {
        const form = modal.querySelector('form');
        const saveBtn = modal.querySelector('.btn-save-modal');

        if (!form || !saveBtn) return;

        saveBtn.addEventListener('click', async (e) => {
            e.preventDefault();

            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }

            const formData = new FormData(form);
            const originalBtnContent = saveBtn.innerHTML;

            try {
                saveBtn.disabled = true;
                saveBtn.querySelector('.spinner-border').classList.remove('d-none');

                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
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
                        modalBody.innerHTML = `<div class="container-fluid p-4">${html}</div>`;
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
        });
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
            input.addEventListener('input', (e) => this.handleSearch(e));
            input.addEventListener('focus', (e) => this.handleFocus(e));
            input.addEventListener('blur', (e) => this.handleBlur(e));
            input.addEventListener('keydown', (e) => this.handleKeyDown(e));
        });

        const newClearBtns = container.querySelectorAll('.reference-clear-btn');
        newClearBtns.forEach(btn => {
            btn.addEventListener('click', (e) => this.clearSelection(e));
        });

        const newCreateBtns = container.querySelectorAll('.reference-create-btn');
        newCreateBtns.forEach(btn => {
            btn.addEventListener('click', (e) => this.openCreateModal(e));
        });

        newRefFields.forEach(input => {
            try {
                const filterConfig = input.dataset.referenceFilters;
                if (!filterConfig || filterConfig === '{}') {
                    return;
                }

                const config = JSON.parse(filterConfig);

                for (const [, filterInfo] of Object.entries(config)) {
                    if (filterInfo.isProperty) {
                        const sourceFieldName = filterInfo.value;
                        const sourceHiddenInput = container.querySelector(`input[name="${sourceFieldName}"]`) ||
                            document.querySelector(`input[name="${sourceFieldName}"]`);

                        if (sourceHiddenInput) {
                            // Buscar o DisplayName do campo de origem
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
                                    input.placeholder = `Selecione o campo ${sourceFieldDisplayName} primeiro`;
                                } else {
                                    input.disabled = false;
                                    input.placeholder = input.dataset.originalPlaceholder || 'Digite para pesquisar...';
                                }
                            });

                            if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                                input.disabled = true;
                                input.dataset.originalPlaceholder = input.placeholder;
                                input.placeholder = `Selecione o campo ${sourceFieldDisplayName} primeiro`;
                            }
                        }
                    }
                }
            } catch (error) {
                console.error('Erro ao configurar observadores de dependência:', error);
            }
        });
    }

    showValidationErrors(form, errors) {
        Object.keys(errors).forEach(key => {
            const input = form.querySelector(`[name="${key}"]`);
            if (input) {
                input.classList.add('is-invalid');

                let feedback = input.parentElement.querySelector('.invalid-feedback');
                if (!feedback) {
                    feedback = document.createElement('div');
                    feedback.className = 'invalid-feedback';
                    input.parentElement.appendChild(feedback);
                }
                feedback.textContent = errors[key];
            }
        });

        if (errors.general) {
            this.showToast(errors.general, 'error');
        }
    }

    showToast(message, type = 'info') {
        const toastContainer = document.querySelector('.toast-container') || this.createToastContainer();

        const toastId = `toast_${Date.now()}`;
        const bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';

        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);

        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
        toast.show();

        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });
    }

    createToastContainer() {
        const container = document.createElement('div');
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
        return container;
    }
}

// Inicializar o gerenciador
const referenceFieldManager = new ReferenceFieldManager();

// Exportar para uso global se necessário
window.referenceFieldManager = referenceFieldManager;