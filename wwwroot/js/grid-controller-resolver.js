class GridControllerResolver {
    constructor() {
        this.cache = new Map();
        this.init();
    }

    init() {
        console.log('🚀 Grid Controller Resolver inicializado - Sistema 100% genérico');
    }

    /**
     * Obtém o nome do controller baseado no caminho atual
     * Convenção: /NomeController/Action -> NomeController
     * @returns {string} Nome do controller
     */
    getCurrentController() {
        const path = window.location.pathname;
        const segments = path.split('/').filter(s => s);

        if (segments.length === 0) {
            console.warn('Não foi possível determinar o controller a partir do path:', path);
            return null;
        }

        const controller = segments[0];
        console.log(`📍 Controller detectado automaticamente: ${controller}`);
        return controller;
    }

    /**
     * Obtém o nome da entidade baseado no controller atual
     * Convenção: NomeController (plural) -> NomeEntidade (singular)
     * Exemplos: Clientes -> Cliente, Veiculos -> Veiculo, VeiculoMarcas -> VeiculoMarca
     * @returns {string} Nome da entidade
     */
    getCurrentEntity() {
        const controller = this.getCurrentController();
        if (!controller) return null;

        // Cache para evitar recálculos
        if (this.cache.has(controller)) {
            return this.cache.get(controller);
        }

        let entity = controller;
        this.cache.set(controller, entity);
        return entity;
    }

    /**
     * Constrói URL para action do controller atual
     * @param {string} action - Nome da action
     * @param {string|number} id - ID opcional
     * @returns {string} URL completa
     */
    buildActionUrl(action, id = null) {
        const controller = this.getCurrentController();
        if (!controller) {
            console.error('Não foi possível determinar o controller para buildActionUrl');
            return '#';
        }

        let url = `/${controller}/${action}`;
        if (id) {
            url += `/${id}`;
        }

        return url;
    }

    /**
     * Constrói URL AJAX para GetDataAjax do controller atual
     * @returns {string} URL para AJAX
     */
    getAjaxUrl() {
        const controller = this.getCurrentController();
        if (!controller) {
            console.error('Não foi possível determinar o controller para getAjaxUrl');
            return null;
        }

        const ajaxUrl = `/${controller}/GetDataAjax`;
        return ajaxUrl;
    }

    /**
     * Constrói URL de exportação para o controller atual
     * @param {string} formato - Formato de exportação (csv, excel, etc)
     * @returns {string} URL de exportação
     */
    getExportUrl(formato = 'csv') {
        return this.buildActionUrl('Export') + `?formato=${formato}`;
    }

    /**
     * Navega para action específica do controller atual
     * @param {string} action - Nome da action
     * @param {string|number} id - ID opcional
     * @param {boolean} showLoading - Se deve mostrar loading
     */
    navigateTo(action, id = null, showLoading = true) {
        const url = this.buildActionUrl(action, id);

        if (showLoading && window.showLoading) {
            window.showLoading(true);
        }

        window.location.href = url;
    }

    /**
     * Métodos de conveniência para ações comuns
     */
    navigateToDetails(id) {
        this.navigateTo('Details', id);
    }

    navigateToEdit(id) {
        this.navigateTo('Edit', id);
    }

    navigateToCreate() {
        this.navigateTo('Create');
    }

    navigateToDelete(id) {
        this.navigateTo('Delete', id);
    }

    navigateToIndex() {
        this.navigateTo('Index');
    }

    /**
     * Confirma exclusão de forma genérica
     * @param {string|number} id - ID do registro
     * @param {string} message - Mensagem personalizada (opcional)
     */
    async confirmDelete(id, message = null) {
        const defaultMessage = 'Tem certeza que deseja excluir este registro? Esta ação não pode ser desfeita.';
        const confirmMessage = message || defaultMessage;

        // Fechar dropdowns se existir o sistema
        if (window.dropdownPortalSystem) {
            window.dropdownPortalSystem.forceClose();
        }

        // Usar sistema de confirmação customizado se existir
        if (window.showConfirm) {
            const confirmed = await window.showConfirm(confirmMessage);
            if (confirmed) {
                this.navigateToDelete(id);
            }
        } else {
            // Fallback para confirm nativo
            if (confirm(confirmMessage)) {
                this.navigateToDelete(id);
            }
        }
    }

    /**
     * Debug: Mostra informações do controller atual
     */
    debug() {
        const controller = this.getCurrentController();
        const entity = this.getCurrentEntity();
        const ajaxUrl = this.getAjaxUrl();

        console.group('🔍 Grid Controller Resolver - Debug Info');
        console.log('Path atual:', window.location.pathname);
        console.log('Controller:', controller);
        console.log('Entidade:', entity);
        console.log('URL AJAX:', ajaxUrl);
        console.log('Cache:', Array.from(this.cache.entries()));
        console.groupEnd();
    }
}

// ===================================================================
// INSTÂNCIA GLOBAL E INTEGRAÇÃO
// ===================================================================

// Criar instância global
window.gridControllerResolver = new GridControllerResolver();

// Expor métodos globais para compatibilidade
window.getCurrentController = () => window.gridControllerResolver.getCurrentController();
window.getCurrentEntity = () => window.gridControllerResolver.getCurrentEntity();
window.getAjaxUrl = () => window.gridControllerResolver.getAjaxUrl();
window.buildActionUrl = (action, id) => window.gridControllerResolver.buildActionUrl(action, id);

// Funções de navegação globais
window.navigateToDetails = (id) => window.gridControllerResolver.navigateToDetails(id);
window.navigateToEdit = (id) => window.gridControllerResolver.navigateToEdit(id);
window.navigateToCreate = () => window.gridControllerResolver.navigateToCreate();
window.navigateToIndex = () => window.gridControllerResolver.navigateToIndex();

// Função de confirmação de exclusão global
window.confirmarExclusao = (id, message) => window.gridControllerResolver.confirmDelete(id, message);

// Compatibilidade com nomes antigos
window.editarRegistro = (controllerIgnorado, id) => window.gridControllerResolver.navigateToEdit(id);
window.visualizarRegistro = (controllerIgnorado, id) => window.gridControllerResolver.navigateToDetails(id);
window.excluirRegistro = (id) => window.gridControllerResolver.confirmDelete(id);

console.log('✅ Grid Controller Resolver carregado - Sistema 100% genérico pronto!');