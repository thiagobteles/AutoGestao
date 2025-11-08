// wwwroot/js/tab-system.js - VERSÃO COMPATÍVEL COM _TabbedForm.cshtml
class TabSystem {
    constructor() {
        this.loadedTabs = new Set();
        this.currentParentController = null;
        this.currentParentId = null;
        this.currentMode = null;
    }

    init() {
        console.log('TabSystem inicializado');
        this.detectParentInfo();
        this.setupEventListeners();
        this.loadActiveTab();
    }

    detectParentInfo() {
        // Detectar controller e ID da URL atual
        const path = window.location.pathname;
        const parts = path.split('/').filter(p => p);

        if (parts.length >= 3) {
            this.currentParentController = parts[0];
            this.currentParentId = parseInt(parts[2]);
        } else if (parts.length >= 2) {
            this.currentParentController = parts[0];
            // Tentar obter ID de um input hidden ou data attribute
            const container = document.querySelector('[data-entity-id]');
            if (container) {
                this.currentParentId = parseInt(container.dataset.entityId);
            } else {
                const idInput = document.querySelector('input[name="Id"]');
                if (idInput) {
                    this.currentParentId = parseInt(idInput.value);
                }
            }
        }

        // Detectar o modo (Create, Edit, Details)
        const container = document.querySelector('[data-mode]');
        if (container) {
            this.currentMode = container.dataset.mode;
        }

        console.log('Parent info detectado:', {
            controller: this.currentParentController,
            id: this.currentParentId,
            mode: this.currentMode
        });
    }

    setupEventListeners() {
        // Listener para cliques nas tabs
        document.addEventListener('click', (e) => {
            const tabButton = e.target.closest('[data-bs-toggle="tab"]');
            if (tabButton) {
                this.handleTabClick(tabButton);
            }
        });

        // Listener para quando uma tab é mostrada (evento do Bootstrap)
        document.addEventListener('shown.bs.tab', (e) => {
            const tabButton = e.target;
            this.handleTabShown(tabButton);
        });
    }

    handleTabClick(tabButton) {
        const tabId = this.getCleanTabId(tabButton);
        console.log('Tab clicada:', tabId);
    }

    handleTabShown(tabButton) {
        const tabId = this.getCleanTabId(tabButton);
        const controller = tabButton.getAttribute('data-controller');
        const lazyLoad = tabButton.getAttribute('data-lazy-load') === 'true';

        console.log('Tab mostrada:', { tabId, controller, lazyLoad });

        // Ignorar tab principal
        if (tabId === 'principal') {
            console.log('Tab principal - não precisa carregar');
            return;
        }

        // Se é lazy load e ainda não foi carregada, carregar agora
        if (lazyLoad && controller && !this.loadedTabs.has(tabId)) {
            this.loadTabContent(tabId, controller);
        }
    }

    loadActiveTab() {
        // Carregar a tab ativa inicial se necessário
        const activeTab = document.querySelector('.nav-link.active[data-bs-toggle="tab"]');
        if (activeTab) {
            const tabId = this.getCleanTabId(activeTab);
            const controller = activeTab.getAttribute('data-controller');
            const lazyLoad = activeTab.getAttribute('data-lazy-load') === 'true';

            // Ignorar tab principal
            if (tabId === 'principal') {
                return;
            }

            if (lazyLoad && controller && !this.loadedTabs.has(tabId)) {
                this.loadTabContent(tabId, controller);
            }
        }
    }

    getCleanTabId(tabButton) {
        // Obtém o ID da tab removendo # e sufixo -content
        const target = tabButton.getAttribute('data-bs-target');
        if (!target) return null;

        return target.replace('#', '').replace('-content', '');
    }

    getTabPaneId(tabId) {
        // Retorna o ID completo da tab-pane (com sufixo -content)
        return `${tabId}-content`;
    }

    async loadTabContent(tabId, controller) {
        const tabPaneId = this.getTabPaneId(tabId);
        const tabPane = document.getElementById(tabPaneId);

        if (!tabPane) {
            console.error('Tab pane não encontrado:', tabPaneId);
            return;
        }

        // Verificar se já está carregando
        if (tabPane.dataset.loading === 'true') {
            console.log('Tab já está carregando:', tabId);
            return;
        }

        try {
            tabPane.dataset.loading = 'true';

            console.log('Carregando tab:', {
                tabId,
                controller,
                parentId: this.currentParentId,
                parentController: this.currentParentController
            });

            // Validações
            if (!this.currentParentId) {
                throw new Error('ID do registro pai não encontrado');
            }

            if (!this.currentParentController) {
                throw new Error('Controller pai não encontrado');
            }

            // Construir URL
            let url = `/TabContent/LoadTab?controller=${encodeURIComponent(controller)}&tab=${encodeURIComponent(tabId)}&parentId=${this.currentParentId}&parentController=${encodeURIComponent(this.currentParentController)}`;

            // Adicionar modo se disponível
            if (this.currentMode) {
                url += `&mode=${encodeURIComponent(this.currentMode)}`;
            }

            console.log('URL de carregamento:', url);

            // Mostrar loading
            this.showLoading(tabPane);

            // Fazer requisição
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Accept': 'text/html'
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Erro na resposta:', {
                    status: response.status,
                    statusText: response.statusText,
                    body: errorText
                });
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            const html = await response.text();

            // Verificar se recebeu HTML válido
            if (!html || html.trim().length === 0) {
                throw new Error('Resposta vazia do servidor');
            }

            // Buscar o wrapper da tab
            const wrapper = tabPane.querySelector('.tab-content-wrapper');
            if (wrapper) {
                wrapper.innerHTML = html;
            } else {
                tabPane.innerHTML = html;
            }

            // Marcar como carregada
            this.loadedTabs.add(tabId);

            console.log('Tab carregada com sucesso:', tabId);

            // Inicializar componentes na tab
            this.initializeTabComponents(tabPane);

        } catch (error) {
            console.error('Erro ao carregar tab:', error);
            this.showError(tabPane, error.message);
        } finally {
            tabPane.dataset.loading = 'false';

            // Remover indicador de loading do botão da tab
            const tabButton = document.querySelector(`[data-bs-target="#${this.getTabPaneId(tabId)}"]`);
            if (tabButton) {
                const loadingSpan = tabButton.querySelector('.tab-loading');
                if (loadingSpan) {
                    loadingSpan.classList.add('d-none');
                }
            }
        }
    }

    showLoading(tabPane) {
        const wrapper = tabPane.querySelector('.tab-content-wrapper') || tabPane;
        wrapper.innerHTML = `
            <div class="d-flex justify-content-center align-items-center py-5" style="min-height: 300px;">
                <div class="text-center">
                    <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                        <span class="visually-hidden">Carregando...</span>
                    </div>
                    <p class="mt-3 text-muted fw-semibold">Carregando conteúdo...</p>
                </div>
            </div>
        `;
    }

    showError(tabPane, message) {
        const wrapper = tabPane.querySelector('.tab-content-wrapper') || tabPane;
        wrapper.innerHTML = `
            <div class="alert alert-danger m-4" role="alert">
                <div class="d-flex align-items-start">
                    <i class="fas fa-exclamation-triangle fa-2x me-3"></i>
                    <div class="flex-grow-1">
                        <h5 class="alert-heading">Erro ao carregar conteúdo</h5>
                        <p class="mb-2">${this.escapeHtml(message)}</p>
                        <hr class="my-3">
                        <div class="d-flex gap-2">
                            <button class="btn btn-sm btn-outline-danger" onclick="location.reload()">
                                <i class="fas fa-redo me-1"></i>
                                Recarregar Página
                            </button>
                            <button class="btn btn-sm btn-outline-secondary" onclick="window.history.back()">
                                <i class="fas fa-arrow-left me-1"></i>
                                Voltar
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, m => map[m]);
    }

    initializeTabComponents(tabPane) {
        // Inicializar tooltips
        const tooltips = tabPane.querySelectorAll('[data-bs-toggle="tooltip"]');
        tooltips.forEach(el => {
            new bootstrap.Tooltip(el);
        });

        // Inicializar popovers
        const popovers = tabPane.querySelectorAll('[data-bs-toggle="popover"]');
        popovers.forEach(el => {
            new bootstrap.Popover(el);
        });

        // Inicializar máscaras se jQuery mask plugin estiver disponível
        if (typeof $.fn.mask !== 'undefined') {
            const $tabPane = $(tabPane);
            $tabPane.find('.cpf-mask').mask('000.000.000-00');
            $tabPane.find('.cnpj-mask').mask('00.000.000/0000-00');
            $tabPane.find('.telefone-mask').mask('(00) 0000-00009');
            $tabPane.find('.cep-mask').mask('00000-000');
            $tabPane.find('.money-mask').mask('#.##0,00', { reverse: true });
        }

        // Disparar evento customizado
        const event = new CustomEvent('tab:loaded', {
            detail: { tabPane },
            bubbles: true
        });
        tabPane.dispatchEvent(event);
    }

    reloadTab(tabId) {
        // Remover do cache de tabs carregadas
        this.loadedTabs.delete(tabId);

        // Obter informações da tab
        const tabPaneId = this.getTabPaneId(tabId);
        const tabButton = document.querySelector(`[data-bs-target="#${tabPaneId}"]`);

        if (tabButton) {
            const controller = tabButton.getAttribute('data-controller');
            if (controller) {
                this.loadTabContent(tabId, controller);
            }
        }
    }

    reloadActiveTab() {
        const activeTab = document.querySelector('.nav-link.active[data-bs-toggle="tab"]');
        if (activeTab) {
            const tabId = this.getCleanTabId(activeTab);
            if (tabId && tabId !== 'principal') {
                this.reloadTab(tabId);
            }
        }
    }
}

// Criar instância global
const tabSystem = new TabSystem();

// Inicializar quando o DOM estiver pronto
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        tabSystem.init();
    });
} else {
    tabSystem.init();
}

// Expor globalmente para uso em outros scripts
window.tabSystem = tabSystem;

// Funções auxiliares globais para compatibilidade
window.loadTabContent = function (tabId, controller, parentId) {
    if (parentId) {
        tabSystem.currentParentId = parentId;
    }
    tabSystem.loadTabContent(tabId, controller);
};

window.reloadTab = function (tabId) {
    tabSystem.reloadTab(tabId);
};

window.reloadActiveTab = function () {
    tabSystem.reloadActiveTab();
};

console.log('tab-system.js carregado com sucesso');