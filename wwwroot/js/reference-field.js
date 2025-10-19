// wwwroot/js/reference-field.js - VERSÃO CORRIGIDA E SIMPLIFICADA
class ReferenceFieldManager {
    constructor() {
        this.cache = new Map();
        this.debounceTimers = new Map();
        this.activeRequests = new Map();
    }

    init() {
        this.setupEventListeners();
        this.initializeAllFields();
    }

    setupEventListeners() {
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.reference-field-container')) {
                this.hideAllDropdowns();
            }
        });
    }

    initializeAllFields() {
        const fields = document.querySelectorAll('.reference-search-input');
        
        fields.forEach((input, index) => {
            if (!input.dataset.initialized) {
                this.initializeField(input);
                input.dataset.initialized = 'true';
            }
        });

        const clearBtns = document.querySelectorAll('.reference-clear-btn');

        clearBtns.forEach((btn, index) => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => {
                    this.clearSelection(e);
                });
                btn.dataset.initialized = 'true';
            }
        });

        const createBtns = document.querySelectorAll('.reference-create-btn');

        createBtns.forEach((btn, index) => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => {
                    this.openCreateModal(e);
                });
                btn.dataset.initialized = 'true';
            }
        });
    }

    initializeField(input) {
        input.addEventListener('input', (e) => {
            this.handleSearch(e);
        });

        input.addEventListener('focus', (e) => {
            this.handleFocus(e);
        });

        input.addEventListener('blur', (e) => {
            this.handleBlur(e);
        });

        input.addEventListener('keydown', (e) => {
            this.handleKeyDown(e);
        });

        this.setupConditionalFilters(input);
    }

    setupConditionalFilters(input) {
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
                                targetHiddenInput.value = '0';
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

                        // Verificar estado inicial
                        if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                            input.disabled = true;
                            input.placeholder = `Selecione ${sourceFieldDisplayName} primeiro`;
                        }
                    }
                }
            }
        } catch (error) {
            showError('❌ Erro ao configurar filtros:')
        }
    }

    getFieldDisplayName(fieldName) {
        const field = document.querySelector(`input[name="${fieldName}"]`);
        if (field) {
            const container = field.closest('.form-group-modern, .form-group');
            if (container) {
                const label = container.querySelector('label');
                if (label) return label.textContent.trim();
            }
        }
        return fieldName;
    }

    handleSearch(event) {
        const input = event.target;
        const searchTerm = input.value.trim();
        const targetField = input.dataset.targetField;

        console.log(`🔍 Busca iniciada: "${searchTerm}"`);

        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);
        if (hiddenInput && hiddenInput.value !== '0') {
            hiddenInput.value = '0';
            input.classList.remove('selected');
        }

        if (this.debounceTimers.has(targetField)) {
            clearTimeout(this.debounceTimers.get(targetField));
        }

        if (searchTerm.length < 2) {
            console.log('⚠️ Termo muito curto, escondendo dropdown');
            this.hideDropdown(input);
            return;
        }

        const timer = setTimeout(() => {
            console.log('⏰ Debounce concluído, executando busca...');
            this.performSearch(input, searchTerm);
        }, 300);

        this.debounceTimers.set(targetField, timer);
    }

    handleFocus(event) {
        const input = event.target;
        if (input.value.trim().length >= 2) {
            console.log('👁️ Campo focado com valor, mostrando resultados');
            this.performSearch(input, input.value.trim());
        }
    }

    handleBlur(event) {
        setTimeout(() => {
            if (!document.activeElement?.closest('.reference-dropdown')) {
                console.log('👁️ Campo perdeu foco, escondendo dropdown');
                this.hideDropdown(event.target);
            }
        }, 200);
    }

    handleKeyDown(event) {
        const input = event.target;
        const dropdown = this.getDropdown(input);

        if (!dropdown || dropdown.style.display === 'none') return;

        const items = dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)');
        const currentIndex = parseInt(input.dataset.selectedIndex || '-1');

        switch (event.key) {
            case 'ArrowDown':
                event.preventDefault();
                this.selectNextItem(input, items, currentIndex);
                break;
            case 'ArrowUp':
                event.preventDefault();
                this.selectPreviousItem(input, items, currentIndex);
                break;
            case 'Enter':
                event.preventDefault();
                if (currentIndex >= 0 && currentIndex < items.length) {
                    items[currentIndex].click();
                }
                break;
            case 'Escape':
                event.preventDefault();
                this.hideDropdown(input);
                break;
        }
    }

    selectNextItem(input, items, currentIndex) {
        const newIndex = Math.min(currentIndex + 1, items.length - 1);
        this.highlightItem(input, items, newIndex);
    }

    selectPreviousItem(input, items, currentIndex) {
        const newIndex = Math.max(currentIndex - 1, 0);
        this.highlightItem(input, items, newIndex);
    }

    highlightItem(input, items, index) {
        items.forEach((item, i) => {
            item.classList.toggle('active', i === index);
        });
        input.dataset.selectedIndex = index.toString();

        if (items[index]) {
            items[index].scrollIntoView({ block: 'nearest' });
        }
    }

    async performSearch(input, searchTerm) {
        const targetField = input.dataset.targetField;
        const referenceType = input.dataset.referenceType;

        console.log(`🌐 Iniciando busca na API: ${referenceType} - "${searchTerm}"`);

        if (this.activeRequests.has(targetField)) {
            console.log('⏸️ Abortando requisição anterior');
            this.activeRequests.get(targetField).abort();
        }

        const controller = new AbortController();
        this.activeRequests.set(targetField, controller);

        try {
            const cacheKey = `${referenceType}_${searchTerm}`;
            const cachedResults = this.cache.get(cacheKey);
            let results;

            if (cachedResults) {
                console.log('💾 Resultados encontrados no cache');
                results = cachedResults;
            } else {
                const filters = this.getFiltersForField(input);
                console.log('📤 Enviando requisição:', { referenceType, searchTerm, filters });

                const response = await fetch('/api/Reference/Search', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        entityType: referenceType,
                        searchTerm: searchTerm,
                        pageSize: 10,
                        filters: filters
                    }),
                    signal: controller.signal
                });

                if (!response.ok) {
                    throw new Error(`Erro na busca: ${response.status}`);
                }

                results = await response.json();
                console.log(`✅ Busca concluída: ${results.length} resultados`);

                this.cache.set(cacheKey, results);
                this.activeRequests.delete(targetField);
            }

            this.displayResults(input, results);

        } catch (error) {
            if (error.name !== 'AbortError') {
                console.error('❌ Erro na busca:', error);
                this.showDropdownError(input, 'Erro ao buscar dados');
            }
        }
    }

    getFiltersForField(input) {
        try {
            const filterConfig = input.dataset.referenceFilters;
            if (!filterConfig || filterConfig === '{}') return {};

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

            console.log('🔍 Filtros aplicados:', filters);
            return filters;
        } catch (error) {
            console.error('❌ Erro ao obter filtros:', error);
            return {};
        }
    }

    displayResults(input, results) {
        const dropdown = this.getDropdown(input);

        if (!results || results.length === 0) {
            console.log('ℹ️ Nenhum resultado encontrado');
            dropdown.innerHTML = '<div class="reference-dropdown-item disabled">Nenhum resultado encontrado</div>';
            dropdown.style.display = 'block';
            return;
        }

        console.log(`📋 Exibindo ${results.length} resultados`);
        dropdown.innerHTML = results.map((item, index) => `
            <div class="reference-dropdown-item" data-id="${item.value}" data-text="${item.text}" data-index="${index}">
                <div><strong>${item.text}</strong></div>
                ${item.subtitle ? `<small class="text-muted">${item.subtitle}</small>` : ''}
            </div>
        `).join('');

        dropdown.style.display = 'block';
        input.dataset.selectedIndex = '-1';

        dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)').forEach(item => {
            item.addEventListener('click', (e) => this.selectItem(e, input));
        });
    }

    showDropdownError(input, message) {
        const dropdown = this.getDropdown(input);
        dropdown.innerHTML = `<div class="reference-dropdown-item disabled text-danger">${message}</div>`;
        dropdown.style.display = 'block';
    }

    selectItem(event, input) {
        const item = event.currentTarget;
        const id = item.dataset.id;
        const text = item.dataset.text;
        const targetField = input.dataset.targetField;

        console.log(`✅ Item selecionado: ${text} (${id})`);

        input.value = text;
        input.classList.add('selected');

        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);
        if (hiddenInput) {
            hiddenInput.value = id;
            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            console.log(`💾 Valor salvo: ${targetField} = ${id}`);
        }

        this.hideDropdown(input);
        input.blur();
    }

    clearSelection(event) {
        event.preventDefault();
        event.stopPropagation();

        console.log('🗑️ Limpando seleção...');

        const btn = event.target.closest('.reference-clear-btn');
        const targetField = btn.id.replace('_clear', '');
        const searchInput = document.querySelector(`input[id="${targetField}_search"]`);
        const hiddenInput = document.querySelector(`input[name="${targetField}"]`);

        if (hiddenInput && searchInput) {
            searchInput.value = '';
            hiddenInput.value = '0';
            searchInput.classList.remove('selected');
            this.hideDropdown(searchInput);

            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            searchInput.focus();

            console.log(`✅ Campo limpo: ${targetField}`);
        }
    }

    hideDropdown(input) {
        const dropdown = this.getDropdown(input);
        dropdown.style.display = 'none';
        input.dataset.selectedIndex = '-1';
    }

    hideAllDropdowns() {
        document.querySelectorAll('.reference-dropdown').forEach(dropdown => {
            dropdown.style.display = 'none';
        });
    }

    getDropdown(input) {
        const targetField = input.dataset.targetField;
        let dropdown = document.querySelector(`#${targetField}_dropdown`);

        if (!dropdown) {
            dropdown = document.createElement('div');
            dropdown.id = `${targetField}_dropdown`;
            dropdown.className = 'reference-dropdown';
            dropdown.style.display = 'none';
            input.closest('.reference-field-container').appendChild(dropdown);
        }

        return dropdown;
    }

    async openCreateModal(event) {
        event.preventDefault();
        event.stopPropagation();

        console.log('➕ Abrindo modal de criação...');

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

            console.log('✅ Modal aberto');

        } catch (error) {
            console.error('❌ Erro ao abrir modal:', error);
            this.showError('Erro ao abrir modal de criação');
        }
    }

    getControllerName(referenceType) {
        return referenceType;
    }

    createModal(controller, referenceType, targetField) {
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = 'referenceCreateModal';
        modal.tabIndex = -1;
        modal.innerHTML = `
            <div class="modal-dialog modal-xl modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-plus me-2"></i>
                            Criar ${referenceType}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="text-center py-5">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Carregando...</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        modal.dataset.targetField = targetField;
        return modal;
    }

    async loadCreateContent(modal, controller) {
        try {
            const response = await fetch(`/${controller}/Create`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) {
                throw new Error('Erro ao carregar formulário');
            }

            const html = await response.text();
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            this.setupModalForm(modal);
        } catch (error) {
            console.error('❌ Erro ao carregar conteúdo:', error);
            throw error;
        }
    }

    setupModalForm(modal) {
        const form = modal.querySelector('.standard-form, form');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            await this.handleModalSubmit(modal, form);
        });

        if (typeof window.initializeMasks === 'function') {
            window.initializeMasks();
        }

        if (typeof window.initializeConditionalFields === 'function') {
            window.initializeConditionalFields();
        }

        this.initializeAllFields();
    }

    async handleModalSubmit(modal, form) {
        try {
            const formData = new FormData(form);
            const action = form.action;

            const response = await fetch(action, {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: formData
            });

            if (!response.ok) {
                throw new Error('Erro ao salvar registro');
            }

            const result = await response.json();

            if (result.success) {
                const targetField = modal.dataset.targetField;
                const searchInput = document.querySelector(`input[id="${targetField}_search"]`);
                const hiddenInput = document.querySelector(`input[name="${targetField}"]`);

                if (searchInput && hiddenInput) {
                    searchInput.value = result.text || result.name;
                    searchInput.classList.add('selected');
                    hiddenInput.value = result.id;
                    hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
                }

                bootstrap.Modal.getInstance(modal).hide();
                showSuccess(result.message || 'Registro criado com sucesso!');
            } else {
                this.showValidationErrors(form, result.errors);
            }
        } catch (error) {
            showError('Erro ao salvar registro');
        }
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

        showError('Por favor, corrija os erros no formulário');
    }
}

// Inicialização
console.log('📦 Módulo reference-field.js carregado');

const referenceFieldManager = new ReferenceFieldManager();

if (document.readyState === 'loading') {
    console.log('⏳ Aguardando DOMContentLoaded...');
    document.addEventListener('DOMContentLoaded', () => {
        console.log('✅ DOM carregado, inicializando...');
        referenceFieldManager.init();
    });
} else {
    console.log('✅ DOM já carregado, inicializando imediatamente...');
    referenceFieldManager.init();
}

window.referenceFieldManager = referenceFieldManager;
console.log('✅ referenceFieldManager disponível globalmente');