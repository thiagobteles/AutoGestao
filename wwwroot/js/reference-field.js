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

    initializeAllFields(context = document) {
        console.log('🔧 Inicializando campos de referência...', context === document ? 'documento completo' : 'contexto específico');

        const fields = context.querySelectorAll('.reference-search-input');

        console.log(`📝 Encontrados ${fields.length} campos de referência`);

        fields.forEach((input, index) => {
            if (!input.dataset.initialized) {
                console.log(`✨ Inicializando campo: ${input.id}`);
                this.initializeField(input);
                input.dataset.initialized = 'true';
            }
        });

        const clearBtns = context.querySelectorAll('.reference-clear-btn');

        clearBtns.forEach((btn, index) => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => {
                    this.clearSelection(e);
                });
                btn.dataset.initialized = 'true';
            }
        });

        const createBtns = context.querySelectorAll('.reference-create-btn');

        createBtns.forEach((btn, index) => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => {
                    this.openCreateModal(e);
                });
                btn.dataset.initialized = 'true';
            }
        });

        const searchBtns = context.querySelectorAll('.reference-search-btn');

        searchBtns.forEach((btn, index) => {
            if (!btn.dataset.initialized) {
                btn.addEventListener('click', (e) => {
                    this.openSearchModal(e);
                });
                btn.dataset.initialized = 'true';
            }
        });

        console.log('✅ Inicialização de campos de referência concluída');
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

            // Obter contexto do campo (modal ou página principal)
            const context = input.closest('.modal') || document;

            for (const [, filterInfo] of Object.entries(config)) {
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value;
                    const sourceHiddenInput = this.getFieldInContext(context, sourceFieldName, 'name');

                    if (sourceHiddenInput && !sourceHiddenInput.dataset.listenerAttached) {
                        const sourceFieldDisplayName = this.getFieldDisplayName(sourceFieldName, context);
                        sourceHiddenInput.addEventListener('change', () => {

                            const targetField = input.dataset.targetField;
                            const targetHiddenInput = this.getFieldInContext(context, targetField, 'name');

                            if (targetHiddenInput && targetHiddenInput.value && targetHiddenInput.value !== '0') {
                                input.value = '';
                                targetHiddenInput.value = '0';
                                input.classList.remove('selected');
                                this.hideDropdown(input);
                            }

                            // Obter os botões associados ao campo
                            const createBtn = this.getFieldInContext(context, `${targetField}_create`, 'id');
                            const clearBtn = this.getFieldInContext(context, `${targetField}_clear`, 'id');
                            const searchBtn = this.getFieldInContext(context, `${targetField}_search_all`, 'id');

                            if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                                // Desabilitar campo e botões
                                input.disabled = true;
                                input.placeholder = `Primeiramente selecione o campo ${sourceFieldDisplayName}`;

                                if (createBtn) {
                                    createBtn.disabled = true;
                                    createBtn.style.opacity = '0.5';
                                    createBtn.style.cursor = 'not-allowed';
                                    createBtn.title = `Primeiro selecione ${sourceFieldDisplayName}`;
                                }
                                if (clearBtn) {
                                    clearBtn.disabled = true;
                                    clearBtn.style.opacity = '0.5';
                                    clearBtn.style.cursor = 'not-allowed';
                                }
                                if (searchBtn) {
                                    searchBtn.disabled = true;
                                    searchBtn.style.opacity = '0.5';
                                    searchBtn.style.cursor = 'not-allowed';
                                    searchBtn.title = `Primeiro selecione ${sourceFieldDisplayName}`;
                                }
                            } else {
                                // Habilitar campo e botões
                                input.disabled = false;
                                input.placeholder = input.dataset.originalPlaceholder || 'Digite para pesquisar...';

                                if (createBtn) {
                                    createBtn.disabled = false;
                                    createBtn.style.opacity = '1';
                                    createBtn.style.cursor = 'pointer';
                                    createBtn.title = 'Criar novo registro';
                                }
                                if (clearBtn) {
                                    clearBtn.disabled = false;
                                    clearBtn.style.opacity = '1';
                                    clearBtn.style.cursor = 'pointer';
                                }
                                if (searchBtn) {
                                    searchBtn.disabled = false;
                                    searchBtn.style.opacity = '1';
                                    searchBtn.style.cursor = 'pointer';
                                    searchBtn.title = 'Buscar registros existentes';
                                }
                            }
                        });

                        sourceHiddenInput.dataset.listenerAttached = 'true';

                        // Verificar estado inicial e desabilitar campo e botões se necessário
                        const targetField = input.dataset.targetField;
                        const createBtn = this.getFieldInContext(context, `${targetField}_create`, 'id');
                        const clearBtn = this.getFieldInContext(context, `${targetField}_clear`, 'id');
                        const searchBtn = this.getFieldInContext(context, `${targetField}_search_all`, 'id');

                        if (!sourceHiddenInput.value || sourceHiddenInput.value === '0') {
                            input.disabled = true;
                            input.placeholder = `Primeiramente selecione o campo ${sourceFieldDisplayName}`;

                            if (createBtn) {
                                createBtn.disabled = true;
                                createBtn.style.opacity = '0.5';
                                createBtn.style.cursor = 'not-allowed';
                                createBtn.title = `Primeiro selecione ${sourceFieldDisplayName}`;
                            }
                            if (clearBtn) {
                                clearBtn.disabled = true;
                                clearBtn.style.opacity = '0.5';
                                clearBtn.style.cursor = 'not-allowed';
                            }
                            if (searchBtn) {
                                searchBtn.disabled = true;
                                searchBtn.style.opacity = '0.5';
                                searchBtn.style.cursor = 'not-allowed';
                                searchBtn.title = `Primeiro selecione ${sourceFieldDisplayName}`;
                            }
                        }
                    }
                }
            }
        } catch (error) {
            showError('❌ Erro ao configurar filtros:')
        }
    }

    getFieldDisplayName(fieldName, context = document) {
        const field = this.getFieldInContext(context, fieldName, 'name');
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

        // Obter contexto do campo
        const context = input.closest('.modal') || document;
        const hiddenInput = this.getFieldInContext(context, targetField, 'name');

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

            // Obter contexto do campo
            const context = input.closest('.modal') || document;

            for (const [filterField, filterInfo] of Object.entries(config)) {
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value;
                    const sourceInput = this.getFieldInContext(context, sourceFieldName, 'name');

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

        // Obter contexto do campo
        const context = input.closest('.modal') || document;
        const hiddenInput = this.getFieldInContext(context, targetField, 'name');

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

        // Obter contexto do botão
        const context = btn.closest('.modal') || document;
        const searchInput = this.getFieldInContext(context, `${targetField}_search`, 'id');
        const hiddenInput = this.getFieldInContext(context, targetField, 'name');

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
            const controller = referenceType;
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

    async openSearchModal(event) {
        event.preventDefault();
        event.stopPropagation();

        console.log('🔍 Abrindo modal de busca...');

        const btn = event.target.closest('.reference-search-btn');
        const referenceType = btn.dataset.referenceType;
        const targetField = btn.dataset.targetField;

        try {
            const controller = referenceType;
            const modal = this.createSearchModal(controller, referenceType, targetField);
            document.body.appendChild(modal);

            await this.loadSearchContent(modal, controller, btn);

            const bsModal = new bootstrap.Modal(modal, {
                backdrop: 'static',
                keyboard: true
            });
            bsModal.show();

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });

            console.log('✅ Modal de busca aberto');

        } catch (error) {
            console.error('❌ Erro ao abrir modal de busca:', error);
            this.showError('Erro ao abrir modal de busca');
        }
    }

    createSearchModal(controller, referenceType, targetField) {
        // Detectar nível do modal atual
        const currentModal = event?.target?.closest('.modal');
        const currentLevel = currentModal ? parseInt(currentModal.dataset.modalLevel || '0') : 0;
        const newLevel = currentLevel + 1;

        console.log(`📊 Criando modal de busca nível ${newLevel} (pai: ${currentLevel})`);

        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = `referenceSearchModal_${newLevel}_${Date.now()}`;
        modal.tabIndex = -1;
        modal.dataset.modalLevel = newLevel;
        modal.dataset.targetField = targetField;
        modal.innerHTML = `
            <div class="modal-dialog modal-xl modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-search me-2"></i>
                            Buscar ${referenceType}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" data-modal-context="${newLevel}">
                        <div class="mb-3">
                            <input type="text"
                                   class="form-control"
                                   id="searchModalInput_${newLevel}"
                                   placeholder="Digite para filtrar resultados...">
                        </div>
                        <div id="searchModalResults_${newLevel}">
                            <div class="text-center py-5">
                                <div class="spinner-border text-primary" role="status">
                                    <span class="visually-hidden">Carregando...</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer bg-light">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>Fechar
                        </button>
                    </div>
                </div>
            </div>
        `;
        return modal;
    }

    async loadSearchContent(modal, controller, btn) {
        try {
            const context = btn.closest('.modal') || document;
            const targetField = btn.dataset.targetField;
            const searchInput = this.getFieldInContext(context, `${targetField}_search`, 'id');

            // Obter filtros configurados
            let filters = {};
            if (searchInput && searchInput.dataset.referenceFilters) {
                const filterConfig = JSON.parse(searchInput.dataset.referenceFilters);

                for (const [filterFieldName, filterInfo] of Object.entries(filterConfig)) {
                    if (filterInfo.isProperty) {
                        const sourceFieldName = filterInfo.value;
                        const sourceHiddenInput = this.getFieldInContext(context, sourceFieldName, 'name');
                        if (sourceHiddenInput && sourceHiddenInput.value && sourceHiddenInput.value !== '0') {
                            filters[filterFieldName] = sourceHiddenInput.value;
                        }
                    } else {
                        filters[filterFieldName] = filterInfo.value;
                    }
                }
            }

            // Fazer requisição para buscar registros
            const queryParams = new URLSearchParams(filters);
            const response = await fetch(`/${controller}/Search?${queryParams}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) {
                throw new Error('Erro ao buscar registros');
            }

            const data = await response.json();
            this.renderSearchResults(modal, data, targetField);

            // Adicionar filtro local
            const modalLevel = modal.dataset.modalLevel;
            const searchModalInput = modal.querySelector(`#searchModalInput_${modalLevel}`);
            if (searchModalInput) {
                searchModalInput.addEventListener('input', (e) => {
                    this.filterSearchResults(modal, e.target.value);
                });
            }

            console.log('✅ Conteúdo de busca carregado');

        } catch (error) {
            console.error('❌ Erro ao carregar conteúdo de busca:', error);
            const modalLevel = modal.dataset.modalLevel;
            const resultsContainer = modal.querySelector(`#searchModalResults_${modalLevel}`);
            resultsContainer.innerHTML = `
                <div class="alert alert-danger">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    Erro ao carregar registros. Por favor, tente novamente.
                </div>
            `;
        }
    }

    renderSearchResults(modal, data, targetField) {
        const modalLevel = modal.dataset.modalLevel;
        const resultsContainer = modal.querySelector(`#searchModalResults_${modalLevel}`);

        if (!data || data.length === 0) {
            resultsContainer.innerHTML = `
                <div class="alert alert-info">
                    <i class="fas fa-info-circle me-2"></i>
                    Nenhum registro encontrado.
                </div>
            `;
            return;
        }

        const resultsHtml = data.map(item => {
            const displayText = item.displayText || item.nome || item.descricao || `ID: ${item.id}`;
            const subtitle = item.subtitle || '';

            return `
                <div class="search-result-item" data-id="${item.id}" data-display-text="${displayText}">
                    <div class="search-result-content">
                        <div class="search-result-title">${displayText}</div>
                        ${subtitle ? `<div class="search-result-subtitle">${subtitle}</div>` : ''}
                    </div>
                    <button type="button" class="btn btn-sm btn-primary search-result-select-btn" title="Selecionar">
                        <i class="fas fa-check"></i>
                    </button>
                </div>
            `;
        }).join('');

        resultsContainer.innerHTML = `<div class="search-results-list">${resultsHtml}</div>`;

        // Adicionar event listeners para os botões de seleção
        resultsContainer.querySelectorAll('.search-result-select-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation(); // Impedir que o clique no botão dispare o duplo clique do item
                const item = e.target.closest('.search-result-item');
                this.selectSearchResult(modal, item, targetField);
            });
        });

        // Adicionar event listener para duplo clique nos itens
        resultsContainer.querySelectorAll('.search-result-item').forEach(item => {
            item.addEventListener('dblclick', (e) => {
                this.selectSearchResult(modal, item, targetField);
            });

            // Adicionar visual feedback ao hover
            item.style.cursor = 'pointer';
        });
    }

    selectSearchResult(modal, item, targetField) {
        const id = item.dataset.id;
        const displayText = item.dataset.displayText;

        // Obter o contexto correto (modal pai ou documento)
        const parentContext = this.getParentContext(modal);

        const searchInput = this.getFieldInContext(parentContext, `${targetField}_search`, 'id');
        const hiddenInput = this.getFieldInContext(parentContext, targetField, 'name');

        if (searchInput && hiddenInput) {
            searchInput.value = displayText;
            hiddenInput.value = id;
            searchInput.classList.add('selected');

            // Disparar evento de mudança
            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));

            // Fechar o modal
            const bsModal = bootstrap.Modal.getInstance(modal);
            bsModal.hide();

            console.log(`✅ Registro selecionado: ${displayText} (ID: ${id})`);
        }
    }

    filterSearchResults(modal, filterText) {
        const modalLevel = modal.dataset.modalLevel;
        const resultsContainer = modal.querySelector(`#searchModalResults_${modalLevel}`);
        const items = resultsContainer.querySelectorAll('.search-result-item');

        const lowerFilter = filterText.toLowerCase();

        items.forEach(item => {
            const text = item.dataset.displayText.toLowerCase();
            if (text.includes(lowerFilter)) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });
    }

    createModal(controller, referenceType, targetField) {
        // Detectar nível do modal atual
        const currentModal = event?.target?.closest('.modal');
        const currentLevel = currentModal ? parseInt(currentModal.dataset.modalLevel || '0') : 0;
        const newLevel = currentLevel + 1;

        console.log(`📊 Criando modal nível ${newLevel} (pai: ${currentLevel})`);

        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = `referenceCreateModal_${newLevel}_${Date.now()}`;
        modal.tabIndex = -1;
        modal.dataset.modalLevel = newLevel;
        modal.dataset.targetField = targetField;
        modal.innerHTML = `
            <div class="modal-dialog modal-reference-create modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-plus me-2"></i>
                            Criar ${referenceType}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body" data-modal-context="${newLevel}">
                        <div class="text-center py-5">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Carregando...</span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer bg-light">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>Cancelar
                        </button>
                        <button type="button" class="btn btn-primary" id="modalSaveBtn_${newLevel}">
                            <i class="fas fa-save me-2"></i>Salvar
                        </button>
                    </div>
                </div>
            </div>
        `;
        return modal;
    }

    async loadCreateContent(modal, controller) {
        try {
            // Adicionar ?modal=true para receber apenas o formulário sem layout
            const response = await fetch(`/${controller}/Create?modal=true`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!response.ok) {
                throw new Error('Erro ao carregar formulário');
            }

            const html = await response.text();
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            console.log('✅ Conteúdo do modal carregado, tamanho:', html.length, 'caracteres');

            // Aguardar um pequeno delay para o navegador processar o HTML
            await new Promise(resolve => setTimeout(resolve, 100));

            this.setupModalForm(modal);
        } catch (error) {
            console.error('❌ Erro ao carregar conteúdo:', error);
            throw error;
        }
    }

    setupModalForm(modal) {
        const form = modal.querySelector('.standard-form, form');
        if (!form) return;

        // Prevenir múltiplas inicializações
        if (form.dataset.modalFormInitialized === 'true') {
            console.log('Formulário modal já inicializado, ignorando duplicação');
            return;
        }

        form.dataset.modalFormInitialized = 'true';

        // Remover listener anterior se existir
        const oldHandler = form._modalSubmitHandler;
        if (oldHandler) {
            form.removeEventListener('submit', oldHandler);
        }

        // Criar novo handler
        const submitHandler = async (e) => {
            e.preventDefault();
            e.stopImmediatePropagation();
            await this.handleModalSubmit(modal, form);
        };

        form._modalSubmitHandler = submitHandler;
        form.addEventListener('submit', submitHandler);

        // Ocultar botões originais do formulário (evitar duplicação)
        const formActions = form.querySelector('.form-actions, .card-footer');
        if (formActions) {
            formActions.style.display = 'none';
        }

        // Wire up modal footer Save button - usar ID com nível do modal
        const modalLevel = modal.dataset.modalLevel || '1';
        const modalSaveBtn = modal.querySelector(`#modalSaveBtn_${modalLevel}`);
        if (modalSaveBtn) {
            modalSaveBtn.addEventListener('click', async () => {
                const originalText = modalSaveBtn.innerHTML;

                try {
                    // Mostrar loading
                    modalSaveBtn.disabled = true;
                    modalSaveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Salvando...';

                    // Submeter o formulário
                    await this.handleModalSubmit(modal, form);
                } catch (error) {
                    // Restaurar botão em caso de erro
                    modalSaveBtn.disabled = false;
                    modalSaveBtn.innerHTML = originalText;
                }
            });
        }

        // Preencher campos dependentes com valores da tela pai
        this.prefillDependentFields(modal);

        console.log('⏳ Aguardando renderização completa do DOM no modal...');

        // Aguardar múltiplos frames para garantir renderização completa
        setTimeout(() => {
            console.log('🎭 Frame 1: Inicializando máscaras...');

            // Inicializar máscaras no modal
            if (typeof window.initializeMasks === 'function') {
                window.initializeMasks();
            }

            setTimeout(() => {
                console.log('🔄 Frame 2: Inicializando campos condicionais...');

                // DEBUG: Ver o HTML real do modal
                const modalBody = modal.querySelector('.modal-body');
                console.log('📄 HTML do modal (primeiros 1000 caracteres):');
                console.log(modalBody?.innerHTML.substring(0, 1000));

                // Verificar se há campos condicionais antes de inicializar
                const conditionalFields = modal.querySelectorAll('[data-conditional-display-rule], [data-conditional-field]');
                console.log(`🔍 Campos condicionais encontrados no modal: ${conditionalFields.length}`);

                // DEBUG: Verificar todos os form-group no modal
                const allFormGroups = modal.querySelectorAll('.form-group');
                console.log(`📦 Total de form-groups no modal: ${allFormGroups.length}`);

                if (allFormGroups.length > 0) {
                    console.log('📋 Primeiros 3 form-groups:');
                    allFormGroups.forEach((group, index) => {
                        if (index < 3) {
                            console.log(`  ${index + 1}. Atributos:`, {
                                'data-field-name': group.dataset.fieldName,
                                'data-conditional-display-rule': group.dataset.conditionalDisplayRule,
                                'data-conditional-field': group.dataset.conditionalField,
                                'data-conditional-value': group.dataset.conditionalValue,
                                'outerHTML (100 chars)': group.outerHTML.substring(0, 100)
                            });
                        }
                    });
                }

                if (conditionalFields.length > 0) {
                    // Log dos campos encontrados
                    conditionalFields.forEach((field, index) => {
                        console.log(`  ${index + 1}. ${field.dataset.fieldName || field.id}:`, {
                            displayRule: field.dataset.conditionalDisplayRule,
                            conditionalField: field.dataset.conditionalField,
                            conditionalValue: field.dataset.conditionalValue
                        });
                    });
                }

                // Inicializar campos condicionais simples
                if (typeof window.initializeConditionalFields === 'function') {
                    window.initializeConditionalFields();
                }

                // Inicializar campos condicionais avançados
                if (typeof window.AdvancedConditionalFields?.initialize === 'function') {
                    window.AdvancedConditionalFields.initialize();
                }

                setTimeout(() => {
                    console.log('📋 Frame 3: Inicializando campos de referência...');

                    // Inicializar campos de referência dentro do modal
                    this.initializeAllFields(modal);

                    console.log('✅ Todas as inicializações do modal concluídas');
                }, 100);
            }, 100);
        }, 150);
    }

    prefillDependentFields(modal) {
        console.log('🔍 Verificando campos dependentes para pré-preencher...');

        try {
            // Obter o campo da tela pai que abriu este modal
            const targetFieldName = modal.dataset.targetField;
            if (!targetFieldName) {
                console.log('⚠️ Modal não tem targetField definido');
                return;
            }

            console.log(`📝 Modal foi aberto pelo campo: ${targetFieldName}`);

            // Obter contexto pai (modal pai ou página principal)
            const parentContext = this.getParentContext(modal);

            // Buscar o campo de referência na tela pai que abriu este modal
            const parentReferenceInput = this.getFieldInContext(parentContext, `${targetFieldName}_search`, 'id');

            if (!parentReferenceInput) {
                console.log(`⚠️ Campo ${targetFieldName}_search não encontrado no contexto pai`);
                return;
            }

            // Ler os filtros configurados no campo da tela pai
            const filterConfig = parentReferenceInput.dataset.referenceFilters;
            if (!filterConfig || filterConfig === '{}') {
                console.log('ℹ️ Campo pai não possui filtros configurados');
                return;
            }

            const config = JSON.parse(filterConfig);
            console.log('📋 Filtros do campo pai:', config);

            // Para cada filtro configurado
            for (const [filterFieldName, filterInfo] of Object.entries(config)) {
                // Só processar filtros que sejam referências a propriedades (isProperty = true)
                if (filterInfo.isProperty) {
                    const sourceFieldName = filterInfo.value; // Nome do campo na tela pai (ex: "IdVeiculoMarca")

                    console.log(`🔄 Processando filtro: ${filterFieldName} ← ${sourceFieldName}`);

                    // Buscar o valor do campo na tela pai
                    const parentHiddenInput = this.getFieldInContext(parentContext, sourceFieldName, 'name');
                    const parentSearchInput = this.getFieldInContext(parentContext, `${sourceFieldName}_search`, 'id');

                    if (!parentHiddenInput || !parentHiddenInput.value || parentHiddenInput.value === '0') {
                        console.log(`⚠️ Campo ${sourceFieldName} na tela pai está vazio ou não encontrado`);
                        continue;
                    }

                    const parentValue = parentHiddenInput.value;
                    const parentDisplayText = parentSearchInput ? parentSearchInput.value : '';

                    console.log(`✅ Valor encontrado na tela pai: ${sourceFieldName} = ${parentValue} (${parentDisplayText})`);

                    // Agora preencher o campo correspondente NO MODAL
                    // O filterFieldName é o nome do campo no modal (ex: "IdVeiculoMarca")
                    const modalSearchInput = this.getFieldInContext(modal, `${filterFieldName}_search`, 'id');
                    const modalHiddenInput = this.getFieldInContext(modal, filterFieldName, 'name');

                    if (!modalSearchInput || !modalHiddenInput) {
                        console.log(`⚠️ Campo ${filterFieldName} não encontrado no modal`);
                        continue;
                    }

                    // Preencher e bloquear o campo no modal
                    modalHiddenInput.value = parentValue;
                    modalSearchInput.value = parentDisplayText;
                    modalSearchInput.classList.add('selected');

                    // Bloquear o campo (readonly)
                    modalSearchInput.readOnly = true;
                    modalSearchInput.disabled = true;
                    modalSearchInput.classList.add('bg-light', 'text-muted');
                    modalSearchInput.style.cursor = 'not-allowed';

                    // Desabilitar botões de adicionar, limpar e buscar
                    const createBtn = modal.querySelector(`#${filterFieldName}_create`);
                    const clearBtn = modal.querySelector(`#${filterFieldName}_clear`);
                    const searchBtn = modal.querySelector(`#${filterFieldName}_search_all`);

                    if (createBtn) {
                        createBtn.style.display = 'none';
                    }

                    if (clearBtn) {
                        clearBtn.style.display = 'none';
                    }

                    if (searchBtn) {
                        searchBtn.style.display = 'none';
                    }

                    // Esconder o btn-group se todos os botões foram escondidos
                    const btnGroup = createBtn?.parentElement;
                    if (btnGroup && btnGroup.classList.contains('btn-group')) {
                        btnGroup.style.display = 'none';
                    }

                    // Adicionar ícone de "cadeado" para indicar que está bloqueado
                    const container = modalSearchInput.closest('.reference-field-container');
                    if (container && !container.querySelector('.field-locked-icon')) {
                        const lockIcon = document.createElement('div');
                        lockIcon.className = 'field-locked-icon';
                        lockIcon.innerHTML = '<i class="fas fa-lock text-muted"></i>';
                        lockIcon.style.cssText = 'position: absolute; right: 15px; top: 50%; transform: translateY(-50%); pointer-events: none; z-index: 10;';

                        if (!container.style.position || container.style.position === 'static') {
                            container.style.position = 'relative';
                        }

                        container.appendChild(lockIcon);
                    }

                    // Adicionar tooltip explicativo
                    modalSearchInput.title = `Campo herdado da tela anterior (não editável)`;

                    console.log(`🔒 Campo ${filterFieldName} no modal bloqueado com valor: ${parentValue} (${parentDisplayText})`);
                }
            }

        } catch (error) {
            console.error('❌ Erro ao pré-preencher campos dependentes:', error);
        }
    }

    async handleModalSubmit(modal, form) {
        const modalLevel = modal.dataset.modalLevel || '1';
        const modalSaveBtn = modal.querySelector(`#modalSaveBtn_${modalLevel}`);
        const originalText = modalSaveBtn?.innerHTML;

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

                // Buscar o contexto correto (modal pai ou página principal)
                const parentContext = this.getParentContext(modal);
                const searchInput = this.getFieldInContext(parentContext, `${targetField}_search`, 'id');
                const hiddenInput = this.getFieldInContext(parentContext, targetField, 'name');

                console.log(`✅ Atualizando campo ${targetField} no contexto pai`, {
                    context: parentContext === document ? 'página principal' : 'modal pai',
                    searchInput: searchInput?.id,
                    hiddenInput: hiddenInput?.name
                });

                if (searchInput && hiddenInput) {
                    searchInput.value = result.text || result.name;
                    searchInput.classList.add('selected');
                    hiddenInput.value = result.id;
                    hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
                }

                bootstrap.Modal.getInstance(modal).hide();
                showSuccess(result.message || 'Registro criado com sucesso!');
            } else {
                // Restaurar botão em caso de erro de validação
                if (modalSaveBtn && originalText) {
                    modalSaveBtn.disabled = false;
                    modalSaveBtn.innerHTML = originalText;
                }
                this.showValidationErrors(form, result.errors);
            }
        } catch (error) {
            // Restaurar botão em caso de erro
            if (modalSaveBtn && originalText) {
                modalSaveBtn.disabled = false;
                modalSaveBtn.innerHTML = originalText;
            }
            showError('Erro ao salvar registro');
            throw error; // Re-throw para o handler no setupModalForm
        }
    }

    // Método auxiliar para obter o contexto pai de um modal
    getParentContext(modal) {
        const currentLevel = parseInt(modal.dataset.modalLevel || '1');

        if (currentLevel <= 1) {
            // Modal de nível 1 - contexto é a página principal
            return document;
        }

        // Modal aninhado - buscar modal pai
        const parentLevel = currentLevel - 1;
        const parentModal = document.querySelector(`.modal[data-modal-level="${parentLevel}"]`);

        if (parentModal) {
            console.log(`🔍 Contexto pai encontrado: modal nível ${parentLevel}`);
            return parentModal;
        }

        // Fallback para documento se não encontrar modal pai
        console.warn(`⚠️ Modal pai nível ${parentLevel} não encontrado, usando documento`);
        return document;
    }

    // Método auxiliar para buscar campo dentro de um contexto específico
    getFieldInContext(context, fieldValue, attributeName = 'name') {
        if (attributeName === 'id') {
            return context.querySelector(`#${fieldValue}`);
        }
        return context.querySelector(`[${attributeName}="${fieldValue}"]`);
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

// ===================================================================
// CSS PARA MODAL DE REFERÊNCIA
// ===================================================================

// Adicionar CSS customizado para o modal de referência
const referenceModalStyles = document.createElement('style');
referenceModalStyles.textContent = `
    /* Modal maior para formulários de referência */
    .modal-reference-create {
        max-width: 90vw !important;
        width: 90vw !important;
        margin: 1.75rem auto;
    }

    @media (min-width: 1200px) {
        .modal-reference-create {
            max-width: 1400px !important;
            width: 1400px !important;
        }
    }

    /* Garantir altura adequada */
    .modal-reference-create .modal-content {
        min-height: 80vh;
        max-height: 90vh;
        display: flex;
        flex-direction: column;
    }

    .modal-reference-create .modal-body {
        flex: 1;
        overflow-y: auto;
        padding: 2rem;
        max-height: calc(90vh - 180px);
    }

    /* Modal footer sempre visível */
    .modal-reference-create .modal-footer {
        position: sticky;
        bottom: 0;
        background: #f8f9fa;
        border-top: 2px solid #dee2e6;
        padding: 1rem 1.5rem;
        z-index: 1000;
        box-shadow: 0 -2px 10px rgba(0,0,0,0.05);
    }

    .modal-reference-create .modal-footer .btn {
        min-width: 120px;
    }

    /* Ocultar elementos de layout se aparecerem no modal */
    #referenceCreateModal .sidebar,
    #referenceCreateModal .navbar-top,
    #referenceCreateModal .main-header,
    #referenceCreateModal .main-footer,
    #referenceCreateModal .content-wrapper > .container-fluid > .row > .col-md-3,
    #referenceCreateModal nav.navbar,
    #referenceCreateModal aside,
    #referenceCreateModal footer {
        display: none !important;
    }

    /* Garantir que o formulário ocupe todo o espaço */
    #referenceCreateModal .standard-form,
    #referenceCreateModal .form-container {
        width: 100% !important;
        max-width: 100% !important;
        margin: 0 !important;
        padding: 0 !important;
    }

    /* Ajustar card do formulário dentro do modal */
    #referenceCreateModal .card {
        border: none !important;
        box-shadow: none !important;
        margin-bottom: 0 !important;
    }

    #referenceCreateModal .card-header {
        display: none !important;
    }

    #referenceCreateModal .card-body {
        padding: 0 !important;
    }

    /* Melhorar espaçamento dos campos no modal */
    #referenceCreateModal .form-group-modern {
        margin-bottom: 1.5rem;
    }

    /* Botões de ação no rodapé do modal */
    #referenceCreateModal .form-actions {
        position: sticky;
        bottom: 0;
        background: white;
        padding: 1rem 0;
        margin-top: 2rem;
        border-top: 1px solid #dee2e6;
        z-index: 10;
    }
`;

document.head.appendChild(referenceModalStyles);
console.log('✅ CSS do modal de referência aplicado');

// CSS adicional para garantir que popups fiquem acima dos modais
const additionalStyles = document.createElement('style');
additionalStyles.textContent = `
    /* Garantir que modais de notificação fiquem acima dos modais de referência */
    #confirmModal,
    #notificationModal {
        z-index: 1070 !important;
    }

    #confirmModal .modal-backdrop,
    #notificationModal .modal-backdrop {
        z-index: 1069 !important;
    }

    /* Quando há múltiplos modais abertos, o backdrop do popup deve ficar acima */
    .modal-backdrop.show {
        z-index: 1055;
    }

    .modal-backdrop.show + .modal-backdrop.show {
        z-index: 1069;
    }
`;

document.head.appendChild(additionalStyles);
console.log('✅ CSS adicional aplicado (z-index para popups)');