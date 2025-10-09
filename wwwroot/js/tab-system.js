class TabSystem {
    constructor() {
        this.currentParentId = 0;
        this.currentParentController = '';
        this.currentMode = '';
        this.loadedTabs = new Set();
        this.loadingTabs = new Set();
        this.init();
    }

    init() {
        this.detectContext();
        this.setupTabEventListeners();
        this.checkTabsAvailability();
    }

    detectContext() {
        // 1. PRIORIDADE: Pegar ID da URL
        const path = window.location.pathname;
        const pathParts = path.split('/').filter(p => p);

        if (pathParts.length >= 3) {
            const possibleId = parseInt(pathParts[2]);
            if (!isNaN(possibleId) && possibleId > 0) {
                this.currentParentId = possibleId;
                console.log('✅ ID detectado da URL:', this.currentParentId);
            }
        }

        // 2. FALLBACK: Tentar pegar do formulário
        if (this.currentParentId === 0) {
            const form = document.querySelector('.standard-form');
            if (form) {
                const idInput = form.querySelector('input[name="Id"]');
                const idValue = parseInt(idInput?.value || 0);
                if (idValue > 0) {
                    this.currentParentId = idValue;
                    console.log('✅ ID detectado do formulário:', this.currentParentId);
                }
            }
        }

        // 3. FALLBACK: Tentar pegar do atributo data-entity-id
        if (this.currentParentId === 0) {
            const container = document.querySelector('[data-entity-id]');
            if (container) {
                const idValue = parseInt(container.dataset.entityId || 0);
                if (idValue > 0) {
                    this.currentParentId = idValue;
                    console.log('✅ ID detectado do data-entity-id:', this.currentParentId);
                }
            }
        }

        // Detectar controller da URL
        if (pathParts.length >= 1) {
            this.currentParentController = pathParts[0];
        }

        // Detectar o modo
        this.currentMode = this.getMode();

        console.log('📋 Contexto detectado:', {
            parentId: this.currentParentId,
            parentController: this.currentParentController,
            mode: this.currentMode,
            url: path
        });
    }

    checkTabsAvailability() {
        if (this.currentParentId === 0) {
            console.log('⚠️ ID = 0: Bloqueando abas até salvar o registro');

            document.querySelectorAll('[data-bs-toggle="tab"]').forEach(button => {
                const tabId = button.dataset.tabId;
                if (tabId !== 'principal') {
                    button.classList.add('disabled');
                    button.setAttribute('disabled', 'disabled');
                    button.style.opacity = '0.5';
                    button.style.cursor = 'not-allowed';
                    button.title = 'Salve o registro antes de acessar esta aba';
                }
            });

            const tabNavigation = document.querySelector('.tab-navigation');
            if (tabNavigation && !document.querySelector('.tabs-disabled-notice')) {
                const notice = document.createElement('div');
                notice.className = 'alert alert-info alert-sm mb-3 tabs-disabled-notice';
                notice.innerHTML = `
                    <i class="fas fa-info-circle me-2"></i>
                    <strong>Atenção:</strong> Salve o registro primeiro para acessar as demais abas.
                `;
                tabNavigation.insertAdjacentElement('afterend', notice);
            }
        } else {
            console.log(`✅ ID = ${this.currentParentId}, Modo = ${this.currentMode}: Abas liberadas para visualização`);

            document.querySelectorAll('[data-bs-toggle="tab"]').forEach(button => {
                button.classList.remove('disabled');
                button.removeAttribute('disabled');
                button.style.opacity = '1';
                button.style.cursor = 'pointer';
                button.title = '';
            });

            const notice = document.querySelector('.tabs-disabled-notice');
            if (notice) {
                notice.remove();
            }
        }
    }

    setupTabEventListeners() {
        document.querySelectorAll('[data-bs-toggle="tab"]').forEach(button => {
            button.addEventListener('shown.bs.tab', (e) => this.handleTabShown(e));

            // NOVO: Adicionar evento de clique para forçar reload
            button.addEventListener('click', (e) => {
                const tabId = e.currentTarget.dataset.tabId;
                if (tabId !== 'principal' && this.loadedTabs.has(tabId)) {
                    console.log('🔄 Forçando reload da tab:', tabId);
                    // Não fazer nada aqui, deixar o shown.bs.tab lidar
                    // Mas podemos limpar o cache se quiser forçar reload
                }
            });
        });
    }

    async handleTabShown(event) {
        const button = event.target;
        const tabId = button.dataset.tabId;
        const controller = button.dataset.controller;
        const lazyLoad = button.dataset.lazyLoad === 'true';

        console.log('👁️ Tab shown:', { tabId, controller, lazyLoad, parentId: this.currentParentId, mode: this.currentMode });

        if (this.currentParentId === 0) {
            console.warn('⚠️ ID inválido - tab não pode carregar');
            return;
        }

        if (tabId === 'principal') {
            console.log('ℹ️ Aba principal - não precisa carregar');
            return;
        }

        if (!lazyLoad) {
            console.log('ℹ️ Tab sem lazy load');
            return;
        }

        // MODIFICADO: Verificar apenas se está carregando
        if (this.loadingTabs.has(tabId)) {
            console.log('⏳ Tab já está sendo carregada:', tabId);
            return;
        }

        // REMOVIDO: A verificação de loadedTabs para permitir reload
        // Se quiser mostrar conteúdo em cache enquanto recarrega:
        if (this.loadedTabs.has(tabId)) {
            console.log('♻️ Tab já carregada, mostrando cache:', tabId);
            // O conteúdo já está lá, apenas retornar
            return;
        }

        if (!controller) {
            console.error('❌ Controller não definido para a tab:', tabId);
            return;
        }

        await this.loadTabContent(tabId, controller);
    }

    async loadTabContent(tabId, controller) {
        // NOVO: Marcar como carregando
        this.loadingTabs.add(tabId);

        const contentDiv = document.querySelector(`#${tabId}-content .tab-content-wrapper`);
        const button = document.querySelector(`#${tabId}-tab`);

        if (!contentDiv) {
            console.error('❌ Container da tab não encontrado:', tabId);
            this.loadingTabs.delete(tabId);
            return;
        }

        try {
            this.setTabLoading(button, true);
            contentDiv.innerHTML = this.getLoadingHtml();

            const mode = this.currentMode;
            const url = `/TabContent/LoadTab?parentController=${this.currentParentController}&parentId=${this.currentParentId}&tabController=${controller}&mode=${mode}`;

            console.log('🔄 Carregando tab via TabContentController:', {
                url,
                tabId,
                controller,
                parentId: this.currentParentId,
                mode
            });

            const response = await fetch(url);

            if (!response.ok) {
                const errorText = await response.text();
                console.error('❌ Erro HTTP:', response.status, errorText);
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const html = await response.text();

            console.log('✅ Tab carregada com sucesso:', tabId);

            contentDiv.innerHTML = html;
            this.loadedTabs.add(tabId);
            this.initializeTabFeatures(tabId);

        } catch (error) {
            console.error('❌ Erro ao carregar tab:', error);
            contentDiv.innerHTML = this.getErrorHtml(error.message);
        } finally {
            this.setTabLoading(button, false);
            // NOVO: Remover do estado de carregamento
            this.loadingTabs.delete(tabId);
        }
    }

    getMode() {
        const path = window.location.pathname.toLowerCase();
        if (path.includes('/create')) return 'Create';
        if (path.includes('/edit')) return 'Edit';
        if (path.includes('/details')) return 'Details';
        return 'Index';
    }

    setTabLoading(button, loading) {
        const spinner = button?.querySelector('.tab-loading');
        if (spinner) {
            spinner.classList.toggle('d-none', !loading);
        }
    }

    initializeTabFeatures(tabId) {
        const tabContent = document.querySelector(`#${tabId}-content`);
        if (!tabContent) return;

        console.log('🔧 Inicializando features da tab:', tabId);

        if (window.initializeStandardForm) {
            window.initializeStandardForm();
        }

        if (window.StandardGrid) {
            new window.StandardGrid();
        }

        if (window.bootstrap) {
            const dropdowns = tabContent.querySelectorAll('[data-bs-toggle="dropdown"]');
            dropdowns.forEach(dropdown => {
                new window.bootstrap.Dropdown(dropdown);
            });
        }
    }

    async refreshTab(tabId) {
        console.log('🔄 Refreshing tab:', tabId);

        // Remover dos dois conjuntos
        this.loadedTabs.delete(tabId);
        this.loadingTabs.delete(tabId);

        const button = document.querySelector(`#${tabId}-tab`);
        const controller = button?.dataset.controller;

        if (controller && button?.classList.contains('active')) {
            await this.loadTabContent(tabId, controller);
        }
    }

    getLoadingHtml() {
        return `
            <div class="text-center py-5">
                <div class="spinner-border text-primary mb-3" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
                <p class="text-muted">Carregando conteúdo...</p>
            </div>
        `;
    }

    getErrorHtml(message) {
        return `
            <div class="alert alert-danger m-4">
                <div class="d-flex align-items-start">
                    <i class="fas fa-exclamation-triangle fa-2x me-3 text-danger"></i>
                    <div>
                        <h5 class="alert-heading">Erro ao carregar conteúdo</h5>
                        <p class="mb-2"><strong>Mensagem:</strong> ${message}</p>
                        <hr>
                        <p class="mb-0 small text-muted">
                            <strong>Possíveis causas:</strong>
                            <ul class="mt-2">
                                <li>Controller não encontrado</li>
                                <li>Entidade não mapeada corretamente</li>
                                <li>Chave estrangeira não configurada</li>
                                <li>Permissões insuficientes</li>
                            </ul>
                        </p>
                    </div>
                </div>
            </div>
        `;
    }
}

// Funções globais continuam iguais...
window.openTabCreateModal = async function (entityType, parentId, foreignKeyProperty, controllerName) {
    console.log('🆕 openTabCreateModal:', { entityType, parentId, foreignKeyProperty, controllerName });

    const modalElement = document.getElementById('tabItemModal');
    const modal = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: false
    });

    const modalBody = document.getElementById('tabItemModalBody');
    const modalTitle = document.getElementById('modalTitleText');
    const submitBtn = document.getElementById('modalSubmitBtn');

    modalTitle.textContent = `Novo ${entityType}`;
    modalBody.innerHTML = tabSystem.getLoadingHtml();

    if (submitBtn) {
        submitBtn.style.display = 'none';
    }

    modal.show();

    try {
        const url = `/${controllerName}/Create?modal=true&${foreignKeyProperty}=${parentId}`;

        console.log('📤 Requisição modal create:', url);

        const response = await fetch(url);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('❌ Erro ao carregar modal:', response.status, errorText);
            throw new Error(`Erro ${response.status}: ${response.statusText}`);
        }

        const contentType = response.headers.get('content-type');
        console.log('📥 Response content-type:', contentType);

        const html = await response.text();

        console.log('✅ HTML recebido, tamanho:', html.length);
        console.log('📄 Primeiros 200 chars:', html.substring(0, 200));

        modalBody.innerHTML = html;

        if (window.initializeStandardForm) {
            window.initializeStandardForm();
        }

        const form = modalBody.querySelector('form');
        if (form) {
            console.log('✅ Formulário encontrado no modal');

            const formButtons = form.querySelectorAll('button[type="submit"], .form-actions');
            formButtons.forEach(btn => btn.style.display = 'none');

            if (submitBtn) {
                submitBtn.style.display = 'inline-block';
                submitBtn.onclick = () => form.requestSubmit();
            }

            form.addEventListener('submit', async (e) => {
                e.preventDefault();
                await handleTabFormSubmit(form, modal);
            });
        } else {
            console.error('❌ Formulário não encontrado no modal');
        }

    } catch (error) {
        console.error('❌ Erro ao abrir modal:', error);
        modalBody.innerHTML = tabSystem.getErrorHtml(error.message);
    }
};

window.openTabEditModal = async function (entityType, itemId, controllerName) {
    console.log('openTabEditModal:', { entityType, itemId, controllerName });

    const modalElement = document.getElementById('tabItemModal');
    const modal = new bootstrap.Modal(modalElement, {
        backdrop: 'static',
        keyboard: false
    });

    const modalBody = document.getElementById('tabItemModalBody');
    const modalTitle = document.getElementById('modalTitleText');
    const submitBtn = document.getElementById('modalSubmitBtn');

    modalTitle.textContent = `Editar ${entityType}`;
    modalBody.innerHTML = tabSystem.getLoadingHtml();

    if (submitBtn) {
        submitBtn.style.display = 'none';
    }

    modal.show();

    try {
        const url = `/${controllerName}/Edit/${itemId}?modal=true`;

        console.log('Carregando modal edit:', url);

        const response = await fetch(url);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Erro ao carregar modal:', response.status, errorText);
            throw new Error(`Erro ${response.status}: ${response.statusText}`);
        }

        const html = await response.text();
        modalBody.innerHTML = html;

        if (window.initializeStandardForm) {
            window.initializeStandardForm();
        }

        const form = modalBody.querySelector('form');
        if (form) {
            const formButtons = form.querySelectorAll('button[type="submit"], .form-actions');
            formButtons.forEach(btn => btn.style.display = 'none');

            if (submitBtn) {
                submitBtn.style.display = 'inline-block';
                submitBtn.onclick = () => form.requestSubmit();
            }

            form.addEventListener('submit', async (e) => {
                e.preventDefault();
                await handleTabFormSubmit(form, modal);
            });
        }

    } catch (error) {
        console.error('Erro ao abrir modal:', error);
        modalBody.innerHTML = tabSystem.getErrorHtml(error.message);
    }
};

window.deleteTabItem = async function (entityType, itemId, controllerName) {
    console.log('deleteTabItem:', { entityType, itemId, controllerName });

    if (!confirm('Tem certeza que deseja excluir este item?')) {
        return;
    }

    try {
        const response = await fetch(`/${controllerName}/Delete/${itemId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();

        if (result.success) {
            showToast('Item excluído com sucesso!', 'success');

            const activeTab = document.querySelector('.nav-link.active');
            const tabId = activeTab?.dataset.tabId;
            if (tabId && tabSystem) {
                await tabSystem.refreshTab(tabId);
            }
        } else {
            showToast(result.message || 'Erro ao excluir item', 'error');
        }

    } catch (error) {
        console.error('Erro ao excluir:', error);
        showToast('Erro ao excluir item', 'error');
    }
};

async function handleTabFormSubmit(form, modal) {
    const formData = new FormData(form);
    const submitBtn = document.getElementById('modalSubmitBtn');

    console.log('Submetendo formulário da tab:', { action: form.action });

    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i> Salvando...';
    }

    try {
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const result = await response.json();

        if (result.success) {
            showToast(result.message || 'Salvo com sucesso!', 'success');

            const modalInstance = bootstrap.Modal.getInstance(document.getElementById('tabItemModal'));
            if (modalInstance) {
                modalInstance.hide();
            }

            const activeTab = document.querySelector('.nav-link.active');
            const tabId = activeTab?.dataset.tabId;
            if (tabId && tabSystem) {
                await tabSystem.refreshTab(tabId);
            }
        } else {
            showToast(result.message || 'Erro ao salvar', 'error');

            form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
            form.querySelectorAll('.invalid-feedback').forEach(el => el.remove());

            if (result.errors) {
                Object.keys(result.errors).forEach(key => {
                    const input = form.querySelector(`[name="${key}"]`);
                    if (input) {
                        input.classList.add('is-invalid');
                        const feedback = document.createElement('div');
                        feedback.className = 'invalid-feedback d-block';
                        feedback.textContent = result.errors[key];
                        input.parentElement.appendChild(feedback);
                    }
                });
            }
        }

    } catch (error) {
        console.error('Erro ao submeter formulário:', error);
        showToast('Erro ao processar solicitação', 'error');
    } finally {
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = '<i class="fas fa-save me-1"></i> Salvar';
        }
    }
}

function showToast(message, type = 'info') {
    if (window.showToast) {
        window.showToast(message, type);
    } else {
        console.log(`[${type.toUpperCase()}] ${message}`);
    }
}

let tabSystem;
if (!window.tabSystem) {
    document.addEventListener('DOMContentLoaded', () => {
        tabSystem = new TabSystem();
        window.tabSystem = tabSystem;
        console.log('✅ Sistema de tabs inicializado com sucesso');
    });
}

window.TabSystem = TabSystem;
window.tabSystem = tabSystem;