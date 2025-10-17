class ModalSystem {
    constructor() {
        this.isInitialized = false;
        this.currentModal = null;
        this.confirmModal = null;
        this.notificationModal = null;
        this.init();
    }

    init() {
        if (this.isInitialized) return;
        
        this.createModalStructures();
        this.setupEventListeners();
        this.isInitialized = true;
        
        console.log('✅ Sistema Completo de Modais inicializado');
    }

    createModalStructures() {
        this.createConfirmModal();
        this.createNotificationModal();
    }

    createConfirmModal() {
        if (document.getElementById('confirmModal')) return;

        const confirmHTML = `
            <!-- Modal de Confirmação -->
            <div class="modal fade" id="confirmModal" tabindex="-1" aria-labelledby="confirmModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-centered confirm-modal-dialog">
                    <div class="modal-content confirm-modal-content">
                        <div class="modal-header confirm-modal-header">
                            <div class="confirm-icon-container">
                                <i class="confirm-icon fas fa-question-circle"></i>
                            </div>
                            <h5 class="modal-title confirm-title" id="confirmModalLabel">Confirmação</h5>
                        </div>
                        <div class="modal-body confirm-modal-body">
                            <p class="confirm-message">Tem certeza que deseja realizar esta ação?</p>
                        </div>
                        <div class="modal-footer confirm-modal-footer">
                            <button type="button" class="btn confirm-btn-cancel" data-bs-dismiss="modal">
                                <i class="fas fa-times me-2"></i>
                                Cancelar
                            </button>
                            <button type="button" class="btn confirm-btn-ok">
                                <i class="fas fa-check me-2"></i>
                                Confirmar
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', confirmHTML);
        this.confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));
    }

    createNotificationModal() {
        if (document.getElementById('notificationModal')) return;

        const notificationHTML = `
            <!-- Modal de Notificação -->
            <div class="modal fade" id="notificationModal" tabindex="-1" aria-labelledby="notificationModalLabel" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-centered notification-modal-dialog">
                    <div class="modal-content notification-modal-content">
                        <div class="modal-header notification-modal-header">
                            <div class="notification-icon-container">
                                <i class="notification-icon fas fa-check"></i>
                            </div>
                            <h5 class="modal-title notification-title" id="notificationModalLabel">Sucesso</h5>
                        </div>
                        <div class="modal-body notification-modal-body">
                            <p class="notification-message">Operação realizada com sucesso!</p>
                        </div>
                        <div class="modal-footer notification-modal-footer">
                            <button type="button" class="btn notification-btn-close" data-bs-dismiss="modal">
                                <i class="fas fa-check me-2"></i>
                                Entendi
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', notificationHTML);
        this.notificationModal = new bootstrap.Modal(document.getElementById('notificationModal'));
    }

    setupEventListeners() {
        // Listeners para modal de confirmação
        const confirmModal = document.getElementById('confirmModal');
        const notificationModal = document.getElementById('notificationModal');
        
        // Cleanup quando modais forem fechados
        confirmModal.addEventListener('hidden.bs.modal', () => {
            this.cleanupConfirm();
        });

        notificationModal.addEventListener('hidden.bs.modal', () => {
            this.cleanupNotification();
        });

        // ESC para fechar
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                if (confirmModal.classList.contains('show')) {
                    this.hideConfirm();
                }
                if (notificationModal.classList.contains('show')) {
                    this.hideNotification();
                }
            }
        });
    }

    // ===================================================================
    // MODAL DE CONFIRMAÇÃO
    // ===================================================================

    confirm(message, options = {}) {
        const modal = document.getElementById('confirmModal');
        const modalContent = modal.querySelector('.confirm-modal-content');
        const title = modal.querySelector('.confirm-title');
        const messageElement = modal.querySelector('.confirm-message');
        const cancelBtn = modal.querySelector('.confirm-btn-cancel');
        const okBtn = modal.querySelector('.confirm-btn-ok');

        // Configurações padrão
        const config = {
            title: 'Confirmação',
            message: message,
            cancelText: 'Cancelar',
            okText: 'Confirmar',
            type: 'warning', // warning, danger, info
            ...options
        };

        // Aplicar configurações
        title.textContent = config.title;
        messageElement.textContent = config.message;
        cancelBtn.innerHTML = `<i class="fas fa-times me-2"></i>${config.cancelText}`;
        okBtn.innerHTML = `<i class="fas fa-check me-2"></i>${config.okText}`;

        // Aplicar estilo por tipo
        modalContent.className = 'modal-content confirm-modal-content';
        modalContent.classList.add(`confirm-${config.type}`);

        // Animação de entrada
        modalContent.style.transform = 'scale(0.8)';
        modalContent.style.opacity = '0';

        // Mostrar modal
        this.confirmModal.show();

        // Animar entrada
        setTimeout(() => {
            modalContent.style.transition = 'all 0.3s ease';
            modalContent.style.transform = 'scale(1)';
            modalContent.style.opacity = '1';
        }, 100);

        // Retornar promise
        return new Promise((resolve) => {
            // Remover listeners anteriores
            const newOkBtn = okBtn.cloneNode(true);
            okBtn.parentNode.replaceChild(newOkBtn, okBtn);
            
            const newCancelBtn = cancelBtn.cloneNode(true);
            cancelBtn.parentNode.replaceChild(newCancelBtn, cancelBtn);

            // Adicionar novos listeners
            newOkBtn.addEventListener('click', () => {
                this.hideConfirm();
                resolve(true);
            });

            newCancelBtn.addEventListener('click', () => {
                this.hideConfirm();
                resolve(false);
            });

            // Auto-focus no botão OK
            modal.addEventListener('shown.bs.modal', () => {
                newOkBtn.focus();
            }, { once: true });
        });
    }

    hideConfirm() {
        if (this.confirmModal) {
            this.confirmModal.hide();
        }
    }

    cleanupConfirm() {
        const modalContent = document.querySelector('.confirm-modal-content');
        if (modalContent) {
            modalContent.style.transform = '';
            modalContent.style.opacity = '';
            modalContent.style.transition = '';
        }
    }

    // ===================================================================
    // MODAL DE NOTIFICAÇÃO (MESMO DO ARQUIVO ANTERIOR)
    // ===================================================================

    notify(message, type = 'success', options = {}) {
        const modal = document.getElementById('notificationModal');
        const modalContent = modal.querySelector('.notification-modal-content');
        const iconContainer = modal.querySelector('.notification-icon-container');
        const icon = modal.querySelector('.notification-icon');
        const title = modal.querySelector('.notification-title');
        const messageElement = modal.querySelector('.notification-message');
        const button = modal.querySelector('.notification-btn-close');

        // Configurações por tipo
        const configs = {
            success: {
                icon: 'fas fa-check-circle',
                title: 'Sucesso!',
                headerClass: 'success',
                buttonText: 'Perfeito!',
                buttonIcon: 'fas fa-thumbs-up'
            },
            error: {
                icon: 'fas fa-exclamation-triangle',
                title: 'Ops! Algo deu errado',
                headerClass: 'error',
                buttonText: 'Entendi',
                buttonIcon: 'fas fa-times'
            },
            warning: {
                icon: 'fas fa-exclamation-circle',
                title: 'Atenção!',
                headerClass: 'warning',
                buttonText: 'OK',
                buttonIcon: 'fas fa-check'
            },
            info: {
                icon: 'fas fa-info-circle',
                title: 'Informação',
                headerClass: 'info',
                buttonText: 'Entendi',
                buttonIcon: 'fas fa-check'
            }
        };

        const config = configs[type] || configs.info;

        // Limpar classes anteriores
        modalContent.className = 'modal-content notification-modal-content';
        iconContainer.className = 'notification-icon-container';

        // Aplicar novo estilo
        modalContent.classList.add(`notification-${config.headerClass}`);
        iconContainer.classList.add(`icon-${config.headerClass}`);

        // Atualizar conteúdo
        icon.className = `notification-icon ${config.icon}`;
        title.textContent = options.title || config.title;
        messageElement.textContent = message;
        button.innerHTML = `<i class="${config.buttonIcon} me-2"></i>${options.buttonText || config.buttonText}`;

        // Animação de entrada
        modalContent.style.transform = 'scale(0.8)';
        modalContent.style.opacity = '0';

        // Mostrar modal
        this.notificationModal.show();

        // Animar entrada
        setTimeout(() => {
            modalContent.style.transition = 'all 0.3s ease';
            modalContent.style.transform = 'scale(1)';
            modalContent.style.opacity = '1';
        }, 100);

        // Auto-focus no botão
        modal.addEventListener('shown.bs.modal', () => {
            button.focus();
        }, { once: true });

        // Retornar promise
        return new Promise((resolve) => {
            modal.addEventListener('hidden.bs.modal', resolve, { once: true });
        });
    }

    hideNotification() {
        if (this.notificationModal) {
            this.notificationModal.hide();
        }
    }

    cleanupNotification() {
        const modalContent = document.querySelector('.notification-modal-content');
        if (modalContent) {
            modalContent.style.transform = '';
            modalContent.style.opacity = '';
            modalContent.style.transition = '';
        }
    }

    // ===================================================================
    // MÉTODOS DE CONVENIÊNCIA
    // ===================================================================

    success(message, options = {}) {
        return this.notify(message, 'success', options);
    }

    error(message, options = {}) {
        return this.notify(message, 'error', options);
    }

    warning(message, options = {}) {
        return this.notify(message, 'warning', options);
    }

    info(message, options = {}) {
        return this.notify(message, 'info', options);
    }

    // Confirmação específica para exclusão
    confirmDelete(itemName = 'este registro') {
        return this.confirm(
            `Tem certeza que deseja excluir ${itemName}? Esta ação não pode ser desfeita.`,
            {
                title: 'Confirmar Exclusão',
                okText: 'Sim, Excluir',
                cancelText: 'Cancelar',
                type: 'danger'
            }
        );
    }
}

// ===================================================================
// INSTÂNCIA GLOBAL E FUNÇÕES DE CONVENIÊNCIA
// ===================================================================

window.ModalSystem = ModalSystem;
window.modalSystem = new ModalSystem();

// Substituir confirm() nativo
window.showConfirm = function(message, options = {}) {
    return window.modalSystem.confirm(message, options);
};

// Substituir alert() nativo (para notificações)
window.showAlert = function(message, type = 'info') {
    return window.modalSystem.notify(message, type);
};

// Funções de notificação
window.showSuccess = function(message, options = {}) {
    return window.modalSystem.success(message, options);
};

window.showError = function(message, options = {}) {
    return window.modalSystem.error(message, options);
};

window.showWarning = function(message, options = {}) {
    return window.modalSystem.warning(message, options);
};

window.showInfo = function(message, options = {}) {
    return window.modalSystem.info(message, options);
};

// Função específica para confirmação de exclusão
window.confirmDelete = function(itemName) {
    return window.modalSystem.confirmDelete(itemName);
};

// ===================================================================
// INTEGRAÇÃO COM SISTEMA DE GRID EXISTENTE
// ===================================================================

// Substituir função de confirmação de exclusão existente
window.confirmarExclusao = async function(id) {
    // Usar novo modal de confirmação
    const confirmed = await confirmDelete();
    
    if (confirmed) {
        const currentPath = window.location.pathname.toLowerCase();
        let controller = '';

        if (currentPath.includes('veiculo')) {
            controller = 'Veiculo';
        }

        if (controller) {
            window.showLoading && window.showLoading(true);
            
            try {
                const response = await fetch(`/${controller}/Delete/${id}`, {
                    method: 'POST'
                });
                
                const result = await response.json();
                window.showLoading && window.showLoading(false);
                
                if (result.sucesso) {
                    await showSuccess('Registro excluído com sucesso!', {
                        title: 'Exclusão Realizada!',
                        buttonText: 'OK'
                    });
                    location.reload();
                } else {
                    await showError('Não foi possível excluir o registro. ' + (result.mensagem || ''), {
                        title: 'Erro na Exclusão',
                        buttonText: 'Entendi'
                    });
                }
            } catch (error) {
                window.showLoading && window.showLoading(false);
                await showError('Erro de conexão durante a exclusão.', {
                    title: 'Erro de Sistema',
                    buttonText: 'Tentar Novamente'
                });
            }
        }
    }
};