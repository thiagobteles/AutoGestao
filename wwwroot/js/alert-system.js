/**
 * Sistema de Alertas Moderno e Elegante
 * Suporta: success, error, warning, info, confirm
 * Com botões customizáveis e callbacks
 */

class AlertSystem {
    constructor() {
        this.container = null;
        this.alerts = new Map();
        this.defaultDuration = 5000;
        this.init();
    }

    init() {
        // Criar container se não existir
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.className = 'alert-container';
            document.body.appendChild(this.container);
        }
    }

    /**
     * Mostrar alerta
     * @param {Object} options - Configurações do alerta
     * @param {string} options.type - Tipo: success, error, warning, info, confirm
     * @param {string} options.title - Título do alerta
     * @param {string} options.message - Mensagem do alerta
     * @param {number} options.duration - Duração em ms (0 = não fecha automaticamente)
     * @param {Array} options.buttons - Array de botões [{text, onClick, primary}]
     * @param {Function} options.onClose - Callback ao fechar
     */
    show(options) {
        const {
            type = 'info',
            title = '',
            message = '',
            duration = this.defaultDuration,
            buttons = [],
            onClose = null
        } = options;

        // Criar elemento do alerta
        const alertId = `alert-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
        const alertEl = this.createAlertElement(alertId, type, title, message, buttons, onClose);

        // Adicionar ao container
        this.container.appendChild(alertEl);
        this.alerts.set(alertId, alertEl);

        // Animar entrada
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                alertEl.classList.add('show');
            });
        });

        // Auto-close se duration > 0
        if (duration > 0 && buttons.length === 0) {
            // Configurar animação da barra de progresso
            alertEl.style.setProperty('--duration', `${duration}ms`);
            alertEl.querySelector('.alert-modern').style.setProperty('animation-duration', `${duration}ms`);

            setTimeout(() => {
                this.close(alertId);
            }, duration);
        }

        return alertId;
    }

    createAlertElement(id, type, title, message, buttons, onClose) {
        const alertWrapper = document.createElement('div');
        alertWrapper.setAttribute('data-alert-id', id);

        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-times-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle',
            confirm: 'fas fa-question-circle'
        };

        const titles = {
            success: 'Sucesso!',
            error: 'Erro!',
            warning: 'Atenção!',
            info: 'Informação',
            confirm: 'Confirmação'
        };

        const alertTitle = title || titles[type];
        const iconClass = icons[type] || icons.info;

        let html = `
            <div class="alert-modern alert-${type}">
                <div class="alert-icon">
                    <i class="${iconClass}"></i>
                </div>
                <div class="alert-content">
                    <h4 class="alert-title">${this.escapeHtml(alertTitle)}</h4>
                    <p class="alert-message">${this.escapeHtml(message)}</p>
        `;

        // Adicionar botões se houver
        if (buttons && buttons.length > 0) {
            html += '<div class="alert-actions">';
            buttons.forEach((btn, index) => {
                const btnClass = btn.primary ? 'alert-btn-primary' : 'alert-btn-secondary';
                const icon = btn.icon ? `<i class="${btn.icon}"></i>` : '';
                html += `
                    <button class="alert-btn ${btnClass}" data-btn-index="${index}">
                        ${icon}
                        ${this.escapeHtml(btn.text || 'OK')}
                    </button>
                `;
            });
            html += '</div>';
        }

        html += `
                </div>
                <button class="alert-close" aria-label="Fechar">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;

        alertWrapper.innerHTML = html;

        // Event listeners
        const closeBtn = alertWrapper.querySelector('.alert-close');
        closeBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            this.close(id);
            if (onClose) onClose();
        });

        // Event listeners para botões de ação
        const actionButtons = alertWrapper.querySelectorAll('.alert-btn');
        actionButtons.forEach((btn) => {
            btn.addEventListener('click', async (e) => {
                e.stopPropagation();
                const btnIndex = parseInt(btn.dataset.btnIndex);
                const button = buttons[btnIndex];

                if (button.onClick) {
                    // Adicionar loading ao botão
                    btn.classList.add('loading');
                    btn.disabled = true;

                    try {
                        const result = await button.onClick();

                        // Se retornar false, não fecha o alerta
                        if (result !== false) {
                            this.close(id);
                        }
                    } catch (error) {
                        console.error('Erro ao executar ação do botão:', error);
                    } finally {
                        btn.classList.remove('loading');
                        btn.disabled = false;
                    }
                } else {
                    this.close(id);
                }
            });
        });

        return alertWrapper;
    }

    close(alertId) {
        const alertEl = this.alerts.get(alertId);
        if (!alertEl) return;

        const alertBox = alertEl.querySelector('.alert-modern');
        alertBox.classList.remove('show');
        alertBox.classList.add('hide');

        setTimeout(() => {
            if (alertEl.parentNode) {
                alertEl.parentNode.removeChild(alertEl);
            }
            this.alerts.delete(alertId);
        }, 400);
    }

    closeAll() {
        this.alerts.forEach((_, id) => this.close(id));
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // ===================================================================
    // MÉTODOS DE ATALHO
    // ===================================================================

    success(message, title = '') {
        return this.show({
            type: 'success',
            title,
            message,
            duration: 4000
        });
    }

    error(message, title = '') {
        return this.show({
            type: 'error',
            title,
            message,
            duration: 6000
        });
    }

    warning(message, title = '') {
        return this.show({
            type: 'warning',
            title,
            message,
            duration: 5000
        });
    }

    info(message, title = '') {
        return this.show({
            type: 'info',
            title,
            message,
            duration: 5000
        });
    }

    /**
     * Diálogo de confirmação
     * @param {string} message - Mensagem
     * @param {Object} options - Opções adicionais
     * @returns {Promise<boolean>} - true se confirmado, false se cancelado
     */
    confirm(message, options = {}) {
        return new Promise((resolve) => {
            this.show({
                type: 'confirm',
                title: options.title || 'Confirmar ação',
                message,
                duration: 0,
                buttons: [
                    {
                        text: options.cancelText || 'Cancelar',
                        primary: false,
                        onClick: () => {
                            resolve(false);
                        }
                    },
                    {
                        text: options.confirmText || 'Confirmar',
                        primary: true,
                        icon: options.confirmIcon || 'fas fa-check',
                        onClick: () => {
                            resolve(true);
                        }
                    }
                ],
                onClose: () => resolve(false)
            });
        });
    }

    /**
     * Diálogo de confirmação de exclusão
     * @param {string} itemName - Nome do item a ser excluído
     * @returns {Promise<boolean>}
     */
    confirmDelete(itemName = 'este registro') {
        return this.confirm(
            `Tem certeza que deseja excluir ${itemName}? Esta ação não pode ser desfeita.`,
            {
                title: 'Confirmar Exclusão',
                confirmText: 'Excluir',
                confirmIcon: 'fas fa-trash',
                cancelText: 'Cancelar'
            }
        );
    }
}

// ===================================================================
// INSTANCIAR E EXPORTAR GLOBALMENTE
// ===================================================================

window.alertSystem = new AlertSystem();

// Criar funções globais para compatibilidade
window.showSuccess = (message, title = '') => window.alertSystem.success(message, title);
window.showError = (message, title = '') => window.alertSystem.error(message, title);
window.showWarning = (message, title = '') => window.alertSystem.warning(message, title);
window.showInfo = (message, title = '') => window.alertSystem.info(message, title);
window.confirm = (message, options) => window.alertSystem.confirm(message, options);
window.confirmDelete = (itemName) => window.alertSystem.confirmDelete(itemName);

console.log('✅ Sistema de Alertas inicializado');
