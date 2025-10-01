/*!
 * Reference Field Handler - VERSÃO ATUALIZADA PARA MODAL GRANDE
 * Sistema completo de busca e criação para campos de referência
 */

class ReferenceFieldHandler {
    constructor() {
        this.debounceTimer = null;
        this.activeDropdown = null;
        this.loadingRequests = new Set();
        this.cache = new Map();
        this.init();
    }

    init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                this.setup();
            });
        } else {
            this.setup();
        }
    }

    setup() {
        this.bindEvents();
        this.loadExistingValues();
        this.setupGlobalListeners();
    }

    bindEvents() {
        // Bind para todos os campos de referência
        document.querySelectorAll('.reference-search-input').forEach(input => {
            input.addEventListener('input', (e) => this.handleSearch(e));
            input.addEventListener('focus', (e) => this.handleFocus(e));
            input.addEventListener('blur', (e) => setTimeout(() => this.handleBlur(e), 200));
            input.addEventListener('keydown', (e) => this.handleKeydown(e));
        });

        // Bind para botões de limpar
        document.querySelectorAll('.reference-clear-btn').forEach(btn => {
            btn.addEventListener('click', (e) => this.clearSelection(e));
        });

        // Bind para botões de criar novo registro
        document.querySelectorAll('.reference-create-btn').forEach(btn => {
            btn.addEventListener('click', (e) => this.openCreateModal(e));
        });
    }

    setupGlobalListeners() {
        // Fechar dropdown ao clicar fora
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.reference-search-wrapper')) {
                this.closeAllDropdowns();
            }
        });

        // Listener para novos elementos adicionados dinamicamente
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        const referenceInputs = node.querySelectorAll('.reference-search-input');
                        referenceInputs.forEach(input => {
                            if (!input.hasAttribute('data-reference-bound')) {
                                this.bindSingleInput(input);
                            }
                        });
                    }
                });
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    bindSingleInput(input) {
        input.setAttribute('data-reference-bound', 'true');
        input.addEventListener('input', (e) => this.handleSearch(e));
        input.addEventListener('focus', (e) => this.handleFocus(e));
        input.addEventListener('blur', (e) => setTimeout(() => this.handleBlur(e), 200));
        input.addEventListener('keydown', (e) => this.handleKeydown(e));

        // Bind botões relacionados
        const wrapper = input.closest('.reference-search-wrapper');
        if (wrapper) {
            const clearBtn = wrapper.querySelector('.reference-clear-btn');
            const createBtn = wrapper.querySelector('.reference-create-btn');

            if (clearBtn && !clearBtn.hasAttribute('data-reference-bound')) {
                clearBtn.setAttribute('data-reference-bound', 'true');
                clearBtn.addEventListener('click', (e) => this.clearSelection(e));
            }

            if (createBtn && !createBtn.hasAttribute('data-reference-bound')) {
                createBtn.setAttribute('data-reference-bound', 'true');
                createBtn.addEventListener('click', (e) => this.openCreateModal(e));
            }
        }
    }

    handleFocus(event) {
        const input = event.target;
        const hiddenInput = document.getElementById(`${input.dataset.targetField}_value`);

        // Se já tem valor selecionado, mostrar na busca para facilitar edição
        if (hiddenInput && hiddenInput.value && !input.value) {
            this.loadDisplayValue(input, hiddenInput.value);
        }
    }

    handleBlur(event) {
        const input = event.target;
        if (this.activeDropdown && this.activeDropdown.style.display !== 'none') {
            // Não fechar se usuário clicou em item do dropdown
            return;
        }
        this.hideDropdown(input);
    }

    handleKeydown(event) {
        const dropdown = this.activeDropdown;
        if (!dropdown || dropdown.style.display === 'none') return;

        const items = dropdown.querySelectorAll('.reference-dropdown-item:not(.disabled)');
        const currentActive = dropdown.querySelector('.reference-dropdown-item.active');
        let currentIndex = Array.from(items).indexOf(currentActive);

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
                this.closeAllDropdowns();
                break;
        }
    }

    highlightItem(items, index) {
        items.forEach((item, i) => {
            item.classList.toggle('active', i === index);
        });
    }

    handleSearch(event) {
        const input = event.target;
        const searchTerm = input.value.trim();
        const referenceType = input.dataset.referenceType;
        const targetField = input.dataset.targetField;

        clearTimeout(this.debounceTimer);

        if (searchTerm.length < 2) {
            this.hideDropdown(input);
            return;
        }

        // Verificar cache primeiro
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

        // Evitar múltiplas requisições simultâneas
        if (this.loadingRequests.has(requestKey)) {
            return;
        }

        this.loadingRequests.add(requestKey);
        this.showLoading(input);

        try {
            const response = await fetch('/api/Reference/Search', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    entityType: referenceType,
                    searchTerm: searchTerm,
                    pageSize: 10
                })
            });

            if (response.ok) {
                const results = await response.json();

                // Cache dos resultados
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

    showLoading(input) {
        const targetField = input.dataset.targetField;
        const dropdown = document.getElementById(`${targetField}_dropdown`);

        dropdown.innerHTML = `
            <div class="dropdown-item disabled text-center py-3">
                <div class="spinner-border spinner-border-sm me-2" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
                Buscando...
            </div>
        `;
        dropdown.style.display = 'block';
        this.activeDropdown = dropdown;
    }

    showError(input, message) {
        const targetField = input.dataset.targetField;
        const dropdown = document.getElementById(`${targetField}_dropdown`);

        dropdown.innerHTML = `
            <div class="dropdown-item disabled text-danger">
                <i class="fas fa-exclamation-triangle me-2"></i>
                ${message}
            </div>
        `;
        dropdown.style.display = 'block';
        this.activeDropdown = dropdown;
    }

    showDropdown(input, results) {
        const targetField = input.dataset.targetField;
        const dropdown = document.getElementById(`${targetField}_dropdown`);

        dropdown.innerHTML = '';

        if (results.length === 0) {
            dropdown.innerHTML = `
                <div class="dropdown-item disabled">
                    <i class="fas fa-search me-2"></i>
                    Nenhum resultado encontrado
                </div>
            `;
        } else {
            results.forEach((item, index) => {
                const div = document.createElement('div');
                div.className = 'dropdown-item reference-dropdown-item';
                if (index === 0) div.classList.add('active');

                div.innerHTML = `
                    <div class="reference-item-content">
                        <div class="reference-item-title">${this.escapeHtml(item.text)}</div>
                        ${item.subtitle ? `<div class="reference-item-subtitle text-muted">${this.escapeHtml(item.subtitle)}</div>` : ''}
                    </div>
                `;

                div.dataset.value = item.value;
                div.dataset.text = item.text;

                div.addEventListener('click', (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    this.selectItem(input, item);
                });

                dropdown.appendChild(div);
            });
        }

        dropdown.style.display = 'block';
        this.activeDropdown = dropdown;
    }

    selectItem(input, item) {
        const targetField = input.dataset.targetField;
        const hiddenInput = document.getElementById(`${targetField}_value`);

        if (hiddenInput) {
            hiddenInput.value = item.value;
            input.value = item.text;
            input.classList.add('selected');

            this.hideDropdown(input);

            // Trigger change events
            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            input.dispatchEvent(new Event('change', { bubbles: true }));

            // Trigger custom event para integrações
            input.dispatchEvent(new CustomEvent('reference:selected', {
                detail: { item: item, targetField: targetField },
                bubbles: true
            }));
        }
    }

    clearSelection(event) {
        event.preventDefault();
        event.stopPropagation();

        const btn = event.target.closest('.reference-clear-btn');
        const wrapper = btn.closest('.reference-search-wrapper');
        const searchInput = wrapper.querySelector('.reference-search-input');
        const hiddenInput = wrapper.querySelector('input[type="hidden"]');

        if (searchInput && hiddenInput) {
            searchInput.value = '';
            hiddenInput.value = '';
            searchInput.classList.remove('selected');

            this.hideDropdown(searchInput);

            // Trigger change events
            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            searchInput.dispatchEvent(new Event('change', { bubbles: true }));

            // Trigger custom event
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
            // Determinar o controller baseado no tipo
            const controller = this.getControllerName(referenceType);

            // Criar o modal
            const modal = this.createModal(controller, referenceType, targetField);
            document.body.appendChild(modal);

            // Carregar o conteúdo do Create
            await this.loadCreateContent(modal, controller);

            // Mostrar o modal
            const bsModal = new bootstrap.Modal(modal, {
                backdrop: 'static',
                keyboard: true
            });
            bsModal.show();

            // Remover o modal quando fechado
            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });

        } catch (error) {
            console.error('Erro ao abrir modal:', error);
            this.showToast('Erro ao abrir modal de criação', 'error');
        }
    }

    getControllerName(referenceType) {
        const controllerMap = {
            'Cliente': 'Clientes',
            'Fornecedor': 'Fornecedores',
            'Vendedor': 'Vendedores',
            'VeiculoMarca': 'VeiculoMarcas',
            'VeiculoMarcaModelo': 'VeiculoMarcaModelos',
            'VeiculoCor': 'VeiculoCores'
        };

        return controllerMap[referenceType] || referenceType + 's';
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

                // Criar container para o formulário
                modalBody.innerHTML = `
                    <div class="container-fluid p-4">
                        ${html}
                    </div>
                `;

                // Configurar o form para submissão AJAX
                this.setupModalForm(modal, controller);

                // Re-inicializar máscaras e validações se necessário
                this.initializeFormElements(modalBody);

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
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            const submitBtn = form.querySelector('button[type="submit"]');
            const originalBtnContent = submitBtn ? submitBtn.innerHTML : '';

            try {
                // Mostrar loading no botão
                if (submitBtn) {
                    submitBtn.innerHTML = `
                        <span class="spinner-border spinner-border-sm me-2" role="status">
                            <span class="visually-hidden">Salvando...</span>
                        </span>
                        Salvando...
                    `;
                    submitBtn.disabled = true;
                }

                const response = await fetch(`/${controller}/Create`, {
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
                            // Fechar modal
                            const bsModal = bootstrap.Modal.getInstance(modal);
                            bsModal.hide();

                            // Selecionar o item recém-criado
                            this.selectNewItem(modal.dataset.targetField, {
                                value: result.id.toString(),
                                text: result.text || result.name || `Item ${result.id}`
                            });

                            // Mostrar sucesso
                            this.showToast('Registro criado com sucesso!', 'success');

                        } else {
                            // Mostrar erros de validação
                            this.showValidationErrors(form, result.errors || {});
                        }
                    } else {
                        // Response HTML - provavelmente erro de validação, recarregar form
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
                // Restaurar botão
                if (submitBtn) {
                    submitBtn.innerHTML = originalBtnContent;
                    submitBtn.disabled = false;
                }
            }
        });
    }

    initializeFormElements(container) {
        // Re-inicializar máscaras, validações, etc. se necessário

        // Exemplo: Re-bind de eventos para campos dinâmicos
        const referenceInputs = container.querySelectorAll('.reference-search-input');
        referenceInputs.forEach(input => {
            if (!input.hasAttribute('data-reference-bound')) {
                this.bindSingleInput(input);
            }
        });
    }

    selectNewItem(targetField, item) {
        const searchInput = document.getElementById(`${targetField}_search`);
        const hiddenInput = document.getElementById(`${targetField}_value`);

        if (searchInput && hiddenInput) {
            hiddenInput.value = item.value;
            searchInput.value = item.text;
            searchInput.classList.add('selected');

            // Trigger events
            hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
            searchInput.dispatchEvent(new Event('change', { bubbles: true }));

            // Trigger custom event
            searchInput.dispatchEvent(new CustomEvent('reference:created', {
                detail: { item: item, targetField: targetField },
                bubbles: true
            }));

            // Limpar cache para forçar nova busca
            this.clearCacheForType(searchInput.dataset.referenceType);
        }
    }

    clearCacheForType(referenceType) {
        // Remove todas as entradas do cache para um tipo específico
        for (let key of this.cache.keys()) {
            if (key.startsWith(`${referenceType}:`)) {
                this.cache.delete(key);
            }
        }
    }

    showValidationErrors(form, errors) {
        // Limpar erros anteriores
        form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
        form.querySelectorAll('.invalid-feedback').forEach(el => el.remove());

        // Mostrar novos erros
        Object.keys(errors).forEach(fieldName => {
            const field = form.querySelector(`[name="${fieldName}"]`);
            if (field) {
                field.classList.add('is-invalid');

                const feedback = document.createElement('div');
                feedback.className = 'invalid-feedback';
                feedback.textContent = errors[fieldName];
                field.parentNode.appendChild(feedback);
            }
        });

        // Scroll para o primeiro erro
        const firstError = form.querySelector('.is-invalid');
        if (firstError) {
            firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
            firstError.focus();
        }
    }

    showToast(message, type = 'info') {
        // Usar SweetAlert2 se disponível
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: type === 'success' ? 'success' : type === 'error' ? 'error' : 'info',
                title: message,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
        // Fallback para toast nativo se disponível
        else if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
            this.createBootstrapToast(message, type);
        }
        // Fallback simples
        else {
            console.log(`Toast ${type}:`, message);
            if (type === 'error') {
                alert(message);
            }
        }
    }

    createBootstrapToast(message, type) {
        const toastContainer = this.getToastContainer();
        const toastId = `toast_${Date.now()}`;

        const bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';
        const iconClass = type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-circle' : 'fa-info-circle';

        const toastHtml = `
            <div id="${toastId}" class="toast ${bgClass} text-white" role="alert">
                <div class="toast-body d-flex align-items-center">
                    <i class="fas ${iconClass} me-2"></i>
                    <span class="flex-grow-1">${message}</span>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);

        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
        toast.show();

        // Remove o toast após ser ocultado
        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });
    }

    getToastContainer() {
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            container.style.zIndex = '1060';
            document.body.appendChild(container);
        }
        return container;
    }

    hideDropdown(input) {
        const targetField = input.dataset.targetField;
        const dropdown = document.getElementById(`${targetField}_dropdown`);
        if (dropdown) {
            dropdown.style.display = 'none';
        }
        this.activeDropdown = null;
    }

    closeAllDropdowns() {
        document.querySelectorAll('.reference-dropdown').forEach(dropdown => {
            dropdown.style.display = 'none';
        });
        this.activeDropdown = null;
    }

    async loadExistingValues() {
        // Carregar valores existentes em modo edição/detalhes
        const hiddenInputs = document.querySelectorAll('input[type="hidden"][id$="_value"]');

        for (const hiddenInput of hiddenInputs) {
            if (hiddenInput.value && hiddenInput.value !== '0') {
                const fieldName = hiddenInput.id.replace('_value', '');
                const searchInput = document.getElementById(`${fieldName}_search`);
                const displaySpan = document.getElementById(`${fieldName}_display`);

                if (searchInput || displaySpan) {
                    const referenceType = searchInput?.dataset.referenceType;
                    if (referenceType) {
                        await this.loadDisplayValue(searchInput || displaySpan, hiddenInput.value, referenceType);
                    }
                }
            }
        }
    }

    async loadDisplayValue(element, id, referenceType) {
        if (!referenceType) {
            referenceType = element.dataset?.referenceType;
        }

        if (!referenceType) return;

        try {
            const response = await fetch('/api/Reference/GetById', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    entityType: referenceType,
                    id: id
                })
            });

            if (response.ok) {
                const item = await response.json();

                if (element.tagName === 'INPUT') {
                    element.value = item.text;
                    element.classList.add('selected');
                } else if (element.tagName === 'SPAN') {
                    element.textContent = item.text;
                }
            } else {
                console.warn(`Não foi possível carregar item ${id} do tipo ${referenceType}`);
                if (element.tagName === 'SPAN') {
                    element.textContent = `ID: ${id}`;
                }
            }
        } catch (error) {
            console.error('Erro ao carregar valor para exibição:', error);
            if (element.tagName === 'SPAN') {
                element.textContent = `ID: ${id}`;
            }
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Método público para limpar cache manualmente
    clearCache() {
        this.cache.clear();
    }

    // Método público para recarregar um valor específico
    async reloadValue(targetField) {
        const hiddenInput = document.getElementById(`${targetField}_value`);
        const searchInput = document.getElementById(`${targetField}_search`);

        if (hiddenInput && hiddenInput.value && searchInput) {
            await this.loadDisplayValue(searchInput, hiddenInput.value);
        }
    }
}

// Inicializar globalmente
window.ReferenceFieldHandler = ReferenceFieldHandler;

// Auto-inicializar
document.addEventListener('DOMContentLoaded', () => {
    if (!window.referenceFieldHandlerInstance) {
        window.referenceFieldHandlerInstance = new ReferenceFieldHandler();
    }
});

// Garantir que funcione também se script for carregado após DOMContentLoaded
if (document.readyState !== 'loading' && !window.referenceFieldHandlerInstance) {
    window.referenceFieldHandlerInstance = new ReferenceFieldHandler();
}

// ===================================================================
// CORREÇÃO DO DROPDOWN DE REFERÊNCIAS
// Adicione este código ao reference-field.js ou carregue separadamente
// ===================================================================

class ReferenceDropdownFix {
    constructor() {
        this.activeDropdown = null;
        this.init();
    }

    init() {
        // Observar quando dropdowns são mostrados
        this.observeDropdowns();

        // Adicionar listeners para ajustar posicionamento
        window.addEventListener('scroll', () => this.repositionActiveDropdown(), true);
        window.addEventListener('resize', () => this.repositionActiveDropdown());
    }

    observeDropdowns() {
        // Usar MutationObserver para detectar quando display muda
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                if (mutation.type === 'attributes' && mutation.attributeName === 'style') {
                    const dropdown = mutation.target;
                    if (dropdown.classList.contains('reference-dropdown')) {
                        const isVisible = dropdown.style.display === 'block';

                        if (isVisible) {
                            this.handleDropdownShow(dropdown);
                        } else {
                            this.handleDropdownHide(dropdown);
                        }
                    }
                }
            });
        });

        // Observar todos os dropdowns
        document.querySelectorAll('.reference-dropdown').forEach(dropdown => {
            observer.observe(dropdown, {
                attributes: true,
                attributeFilter: ['style']
            });
        });
    }

    handleDropdownShow(dropdown) {
        console.log('Dropdown mostrado:', dropdown.id);
        this.activeDropdown = dropdown;

        // SOLUÇÃO 1: Garantir z-index e overflow nos pais
        this.fixContainersOverflow(dropdown);

        // SOLUÇÃO 2: Se ainda não aparecer, usar posicionamento fixo
        setTimeout(() => {
            if (!this.isDropdownVisible(dropdown)) {
                console.warn('Dropdown não visível, aplicando correção de posicionamento...');
                this.useFixedPositioning(dropdown);
            }
        }, 100);
    }

    handleDropdownHide(dropdown) {
        console.log('Dropdown escondido:', dropdown.id);

        // Remover classes de correção
        this.removeContainerFixes(dropdown);

        // Remover posicionamento fixo se foi aplicado
        dropdown.classList.remove('portal-mode');
        dropdown.style.removeProperty('position');
        dropdown.style.removeProperty('top');
        dropdown.style.removeProperty('left');
        dropdown.style.removeProperty('width');

        if (this.activeDropdown === dropdown) {
            this.activeDropdown = null;
        }
    }

    fixContainersOverflow(dropdown) {
        // Adicionar classe aos containers pais para permitir overflow
        let parent = dropdown.parentElement;
        const parentsToFix = [];

        while (parent && parent !== document.body) {
            parentsToFix.push(parent);
            parent = parent.parentElement;
        }

        parentsToFix.forEach(el => {
            el.classList.add('reference-dropdown-parent-active');

            // Forçar overflow visible
            if (window.getComputedStyle(el).overflow === 'hidden' ||
                window.getComputedStyle(el).overflowY === 'hidden') {
                el.style.setProperty('overflow', 'visible', 'important');
            }
        });
    }

    removeContainerFixes(dropdown) {
        // Remover classes dos containers pais
        let parent = dropdown.parentElement;

        while (parent && parent !== document.body) {
            parent.classList.remove('reference-dropdown-parent-active');

            // Remover style inline se foi adicionado por nós
            if (parent.style.overflow === 'visible') {
                parent.style.removeProperty('overflow');
            }

            parent = parent.parentElement;
        }
    }

    isDropdownVisible(dropdown) {
        const rect = dropdown.getBoundingClientRect();
        return rect.height > 0 && rect.width > 0;
    }

    useFixedPositioning(dropdown) {
        const wrapper = dropdown.closest('.reference-search-wrapper');
        if (!wrapper) return;

        const input = wrapper.querySelector('.reference-search-input');
        if (!input) return;

        // Obter posição do input
        const inputRect = input.getBoundingClientRect();

        // Aplicar posicionamento fixo
        dropdown.style.position = 'fixed';
        dropdown.style.top = `${inputRect.bottom + 2}px`;
        dropdown.style.left = `${inputRect.left}px`;
        dropdown.style.width = `${inputRect.width}px`;
        dropdown.style.zIndex = '10000';

        dropdown.classList.add('portal-mode');

        console.log('Aplicado posicionamento fixo:', {
            top: inputRect.bottom + 2,
            left: inputRect.left,
            width: inputRect.width
        });
    }

    repositionActiveDropdown() {
        if (!this.activeDropdown || !this.activeDropdown.classList.contains('portal-mode')) {
            return;
        }

        const wrapper = this.activeDropdown.closest('.reference-search-wrapper');
        if (!wrapper) return;

        const input = wrapper.querySelector('.reference-search-input');
        if (!input) return;

        const inputRect = input.getBoundingClientRect();

        this.activeDropdown.style.top = `${inputRect.bottom + 2}px`;
        this.activeDropdown.style.left = `${inputRect.left}px`;
        this.activeDropdown.style.width = `${inputRect.width}px`;
    }
}

// Inicializar quando o DOM estiver pronto
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.referenceDropdownFix = new ReferenceDropdownFix();
    });
} else {
    window.referenceDropdownFix = new ReferenceDropdownFix();
}