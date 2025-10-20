class GridControllerResolver {
    constructor() {
        this.cache = new Map();
        this.init();
    }

    init() {
        console.log('🚀 Grid Controller Resolver inicializado - Sistema 100% genérico');
    }

    getCurrentController() {
        const path = window.location.pathname;
        const segments = path.split('/').filter(s => s);

        if (segments.length === 0) {
            console.warn('Não foi possível determinar o controller a partir do path:', path);
            return null;
        }

        const controller = segments[0];
        return controller;
    }

    getCurrentEntity() {
        const controller = this.getCurrentController();
        if (!controller) return null;

        if (this.cache.has(controller)) {
            return this.cache.get(controller);
        }

        let entity = controller;
        this.cache.set(controller, entity);
        return entity;
    }

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

    getAjaxUrl() {
        const controller = this.getCurrentController();
        if (!controller) {
            console.error('Não foi possível determinar o controller para getAjaxUrl');
            return null;
        }

        return `/${controller}/GetDataAjax`;
    }

    navigateTo(action, id = null, showLoading = true) {
        const url = this.buildActionUrl(action, id);

        if (showLoading && window.showLoading) {
            window.showLoading(true);
        }

        window.location.href = url;
    }

    navigateToDetails(id) {
        this.navigateTo('Details', id);
    }

    navigateToEdit(id) {
        this.navigateTo('Edit', id);
    }

    navigateToCreate() {
        this.navigateTo('Create');
    }

    navigateToIndex() {
        this.navigateTo('Index');
    }

    async executeDelete(id) {
        const controller = this.getCurrentController();
        if (!controller) {
            console.error('Controller não identificado');
            return;
        }

        window.showLoading && window.showLoading(true);

        try {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : '';

            const response = await fetch(`/${controller}/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: token ? `__RequestVerificationToken=${encodeURIComponent(token)}` : ''
            });

            window.showLoading && window.showLoading(false);

            if (response.ok) {
                const result = await response.json();

                if (result.sucesso || result.success) {
                    if (result.script) {
                        eval(result.script);
                    } else {
                        await showSuccess(result.mensagem || result.message || 'Registro excluído com sucesso!', {
                            title: 'Exclusão Realizada!',
                            buttonText: 'OK'
                        });

                        if (window.responseHandler) {
                            window.responseHandler.isNavigating = true;
                        }

                        setTimeout(() => {
                            const redirectUrl = result.redirectUrl || `/${controller}`;
                            window.location.href = redirectUrl;
                        }, 1500);
                    }
                } else {
                    await showError(result.mensagem || result.message || 'Não foi possível excluir o registro.', {
                        title: 'Erro na Exclusão',
                        buttonText: 'Entendi'
                    });
                }
            } else {
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }

        } catch (error) {
            console.error('Erro ao excluir:', error);
            window.showLoading && window.showLoading(false);

            await showError('Erro de conexão durante a exclusão. Tente novamente.', {
                title: 'Erro de Sistema',
                buttonText: 'Tentar Novamente'
            });
        }
    }

    async confirmDelete(id, message = null) {
        const defaultMessage = 'Tem certeza que deseja excluir este registro? Esta ação não pode ser desfeita.';
        const confirmMessage = message || defaultMessage;

        if (window.dropdownPortalSystem) {
            window.dropdownPortalSystem.forceClose();
        }

        let confirmed = false;

        if (window.showConfirm) {
            confirmed = await window.showConfirm(confirmMessage);
        } else if (window.confirmDelete) {
            confirmed = await window.confirmDelete();
        } else {
            confirmed = confirm(confirmMessage);
        }

        if (confirmed) {
            await this.executeDelete(id);
        }
    }
}

window.gridControllerResolver = new GridControllerResolver();

window.getCurrentController = () => window.gridControllerResolver.getCurrentController();
window.getCurrentEntity = () => window.gridControllerResolver.getCurrentEntity();
window.getAjaxUrl = () => window.gridControllerResolver.getAjaxUrl();
window.buildActionUrl = (action, id) => window.gridControllerResolver.buildActionUrl(action, id);

window.navigateToDetails = (id) => window.gridControllerResolver.navigateToDetails(id);
window.navigateToEdit = (id) => window.gridControllerResolver.navigateToEdit(id);
window.navigateToCreate = () => window.gridControllerResolver.navigateToCreate();
window.navigateToIndex = () => window.gridControllerResolver.navigateToIndex();

window.confirmarExclusao = (id, message) => window.gridControllerResolver.confirmDelete(id, message);

window.editarRegistro = (controllerIgnorado, id) => window.gridControllerResolver.navigateToEdit(id);
window.visualizarRegistro = (controllerIgnorado, id) => window.gridControllerResolver.navigateToDetails(id);
window.excluirRegistro = (id) => window.gridControllerResolver.confirmDelete(id);

console.log('✅ Grid Controller Resolver carregado!');