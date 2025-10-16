/**
 * SISTEMA DE HANDLERS AUTOMÁTICOS PARA RESPOSTAS JSON
 * Processa automaticamente os scripts retornados pelos controllers
 */

class ResponseHandler {
    constructor() {
        this.init();
    }

    init() {
        this.interceptFetch();
        this.interceptForms();
        console.log('✅ Sistema de handlers de resposta inicializado');
    }

    interceptFetch() {
        const originalFetch = window.fetch;
        
        window.fetch = async (...args) => {
            try {
                const response = await originalFetch(...args);
                const clonedResponse = response.clone();
                
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    try {
                        const data = await clonedResponse.json();
                        if (data.script) {
                            this.executeScript(data.script);
                        }
                    } catch (e) {
                        // Ignorar erros de parsing JSON
                    }
                }
                
                return response;
            } catch (error) {
                console.error('Erro no fetch:', error);
                throw error;
            }
        };
    }

    interceptForms() {
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
                            this.executeScript(result.script);
                        }
                        
                        if (result.redirectUrl) {
                            setTimeout(() => {
                                window.location.href = result.redirectUrl;
                            }, 2000);
                        }
                    }
                } catch (error) {
                    console.error('Erro no formulário AJAX:', error);
                    showError('Erro de conexão. Tente novamente.');
                }
            }
        });
    }

    executeScript(script) {
        try {
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

// Instância global
window.responseHandler = new ResponseHandler();

// Helper para AJAX
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
            console.error('Erro na requisição:', error);
            showError('Erro de conexão. Tente novamente.');
            throw error;
        }
    }
};

// Substituir confirmarExclusao global
window.confirmarExclusao = async function(id) {
    const confirmed = await confirmDelete();
    
    if (confirmed) {
        const currentPath = window.location.pathname.toLowerCase();
        let controller = '';

        if (currentPath.includes('veiculos')) {
            controller = 'Veiculos';
        } else if (currentPath.includes('clientes')) {
            controller = 'Clientes';
        } else if (currentPath.includes('vendedores')) {
            controller = 'Vendedores';
        } else if (currentPath.includes('fornecedores')) {
            controller = 'Fornecedores';
        }

        if (controller) {
            window.showLoading && window.showLoading(true);
            
            try {
                await window.ajaxHelper.post(`/${controller}/Delete/${id}`);
                setTimeout(() => {
                    window.location.reload();
                }, 2000);
            } catch (error) {
                // Erro já tratado
            } finally {
                window.showLoading && window.showLoading(false);
            }
        }
    }
};

console.log('🔗 Sistema de handlers automáticos carregado');