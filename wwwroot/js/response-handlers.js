class ResponseHandler {
    constructor() {
        this.isNavigating = false;
        this.init();
    }

    init() {
        this.interceptFetch();
        this.interceptForms();
        this.setupBeforeUnload();
    }

    setupBeforeUnload() {
        window.addEventListener('beforeunload', () => {
            this.isNavigating = true;
        });
    }

    interceptFetch() {
        const originalFetch = window.fetch;
        const self = this;

        window.fetch = async (...args) => {
            try {
                const response = await originalFetch(...args);
                const clonedResponse = response.clone();

                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    try {
                        const data = await clonedResponse.json();
                        if (data.script) {
                            self.executeScript(data.script);
                        }
                    } catch (e) { }
                }

                return response;
            } catch (error) {
                if (self.isNavigating || error.name === 'AbortError' || error.message.includes('aborted')) {
                    throw error;
                }
                console.error('Erro no fetch:', error);
                throw error;
            }
        };
    }

    interceptForms() {
        const self = this;

        document.addEventListener('submit', async (e) => {
            const form = e.target;

            if (form.hasAttribute('data-ajax') || form.classList.contains('ajax-form')) {
                e.preventDefault();

                try {
                    const formData = new FormData(form);
                    const response = await fetch(form.action || window.location.href, {
                        method: form.method || 'POST',
                        body: formData
                    });

                    if (response.ok) {
                        const result = await response.json();

                        if (result.script) {
                            self.executeScript(result.script);
                        }

                        if (result.redirectUrl) {
                            self.isNavigating = true;
                            setTimeout(() => {
                                window.location.href = result.redirectUrl;
                            }, 1500);
                        }
                    }
                } catch (error) {
                    if (!self.isNavigating) {
                        showError('Erro de conexão. Tente novamente.');
                    }
                }
            }
        });
    }

    executeScript(script) {
        try {
            const hasNavigation = script.includes('location.reload') ||
                script.includes('location.href') ||
                script.includes('window.location');

            if (hasNavigation) {
                this.isNavigating = true;
            }

            if (typeof window.modalSystem !== 'undefined') {
                eval(script);
            } else {
                setTimeout(() => {
                    eval(script);
                }, 500);
            }
        } catch (error) {
            console.error('Erro ao executar script:', error);
        }
    }
}

window.responseHandler = new ResponseHandler();

window.ajaxHelper = {
    async post(url, data, options = {}) {
        try {
            const formData = new FormData();

            if (data && typeof data === 'object') {
                for (const [key, value] of Object.entries(data)) {
                    formData.append(key, value);
                }
            }

            const response = await fetch(url, {
                method: 'POST',
                body: formData,
                ...options
            });

            if (response.ok) {
                const result = await response.json();

                if (result.script) {
                    eval(result.script);
                }

                return result;
            } else {
                throw new Error(`Erro ${response.status}`);
            }
        } catch (error) {
            if (!window.responseHandler.isNavigating &&
                error.name !== 'AbortError' &&
                !error.message.includes('aborted')) {
                showError('Erro de conexão. Tente novamente.');
            }
            throw error;
        }
    }
};

window.confirmarExclusao = async function (id) {
    const confirmed = await confirmDelete();

    if (confirmed) {
        const controller = window.gridControllerResolver ?
            window.gridControllerResolver.getCurrentController() :
            null;

        if (controller) {
            window.showLoading && window.showLoading(true);

            try {
                await window.ajaxHelper.post(`/${controller}/Delete/${id}`);

                window.responseHandler.isNavigating = true;
                setTimeout(() => {
                    window.location.href = `/${controller}`;
                }, 1500);
            } catch (error) {
            } finally {
                if (!window.responseHandler.isNavigating) {
                    window.showLoading && window.showLoading(false);
                }
            }
        }
    }
};