/**
 * Sistema de Alertas Moderno e Elegante
 * Suporta: success, error, warning, info, confirm
 * Com botões customizáveis e callbacks
 */

class AlertSystem {
    constructor() {
        this.container = null;
        this.alerts = new Map();
        this.alertPromises = new Map(); // Armazenar promises para cada alerta
        this.defaultDuration = 5000;
        this.init();
    }

    init() {
        // Criar container se não existir
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.className = 'alert-container';
            document.body.appendChild(this.container);

            // Debug detalhado
            console.log('✅ Alert container criado e adicionado ao body');
            console.log('📍 Container position:', {
                parent: this.container.parentElement?.tagName,
                className: this.container.className,
                style: this.container.style.cssText,
                computedStyle: {
                    position: getComputedStyle(this.container).position,
                    zIndex: getComputedStyle(this.container).zIndex,
                    display: getComputedStyle(this.container).display,
                    top: getComputedStyle(this.container).top,
                    right: getComputedStyle(this.container).right
                }
            });
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
     * @returns {Promise} - Promise que resolve quando o alerta é fechado
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

        // Verificar se já existe um alerta com a mesma mensagem e tipo
        const isDuplicate = Array.from(this.alerts.values()).some(alertEl => {
            const existingType = alertEl.querySelector('.alert-modern')?.className.match(/alert-(success|error|warning|info|confirm)/)?.[1];
            const existingMessage = alertEl.querySelector('.alert-message')?.textContent;
            return existingType === type && existingMessage === message;
        });

        if (isDuplicate) {
            console.log(`⚠️ Alerta duplicado ignorado: ${type} - ${message}`);
            // Retornar uma Promise resolvida imediatamente
            return Promise.resolve();
        }

        console.log(`🎨 Mostrando alerta tipo: ${type}, mensagem: ${message}`);

        // Criar elemento do alerta
        const alertId = `alert-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
        const alertEl = this.createAlertElement(alertId, type, title, message, buttons, onClose);

        // Criar Promise que será resolvida quando o alerta fechar
        const promise = new Promise((resolve) => {
            this.alertPromises.set(alertId, resolve);
        });

        console.log('📦 Elemento criado:', {
            id: alertId,
            innerHTML: alertEl.innerHTML.substring(0, 100) + '...',
            className: alertEl.className,
            childElementCount: alertEl.childElementCount
        });

        // Adicionar ao container
        this.container.appendChild(alertEl);
        this.alerts.set(alertId, alertEl);

        console.log('✅ Elemento adicionado ao container. Total de alertas:', this.alerts.size);
        console.log('📊 Container info:', {
            childCount: this.container.children.length,
            isConnected: this.container.isConnected,
            offsetHeight: this.container.offsetHeight,
            offsetWidth: this.container.offsetWidth
        });

        // Animar entrada
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                // IMPORTANTE: Adicionar classe 'show' ao .alert-modern, não ao wrapper
                const alertModern = alertEl.querySelector('.alert-modern');
                if (alertModern) {
                    alertModern.classList.add('show');
                    console.log('🎬 Classe "show" adicionada ao .alert-modern');

                    // Debug do elemento após animação
                    const computedStyle = getComputedStyle(alertModern);
                    console.log('🎨 Estilos do alerta:', {
                        display: computedStyle.display,
                        visibility: computedStyle.visibility,
                        opacity: computedStyle.opacity,
                        transform: computedStyle.transform,
                        position: computedStyle.position
                    });
                } else {
                    console.error('❌ .alert-modern não encontrado dentro do alertEl!');
                }
            });
        });

        // Auto-close se duration > 0
        if (duration > 0 && buttons.length === 0) {
            // Configurar animação da barra de progresso
            alertEl.style.setProperty('--duration', `${duration}ms`);
            const alertModern = alertEl.querySelector('.alert-modern');
            if (alertModern) {
                alertModern.style.setProperty('animation-duration', `${duration}ms`);
            }

            setTimeout(() => {
                this.close(alertId);
            }, duration);
        }

        return promise;
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
        if (alertBox) {
            alertBox.classList.remove('show');
            alertBox.classList.add('hide');
        }

        setTimeout(() => {
            if (alertEl.parentNode) {
                alertEl.parentNode.removeChild(alertEl);
            }
            this.alerts.delete(alertId);

            // Resolver a Promise do alerta
            const resolve = this.alertPromises.get(alertId);
            if (resolve) {
                resolve();
                this.alertPromises.delete(alertId);
            }
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

// Inicializar quando o DOM estiver pronto
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAlertSystem);
} else {
    initAlertSystem();
}

function initAlertSystem() {
    console.log('🚀 Inicializando Alert System...');

    window.alertSystem = new AlertSystem();

    // Criar funções globais para compatibilidade
    window.showSuccess = (message, title = '') => {
        console.log('📞 showSuccess chamado:', message);
        return window.alertSystem.success(message, title);
    };

    window.showError = (message, title = '') => {
        console.log('📞 showError chamado:', message);
        return window.alertSystem.error(message, title);
    };

    window.showWarning = (message, title = '') => {
        console.log('📞 showWarning chamado:', message);
        return window.alertSystem.warning(message, title);
    };

    window.showInfo = (message, title = '') => {
        console.log('📞 showInfo chamado:', message);
        return window.alertSystem.info(message, title);
    };

    // Funções de confirmação (não sobrescrever window.confirm nativo)
    window.showConfirm = (message, options) => {
        console.log('📞 showConfirm chamado:', message);
        return window.alertSystem.confirm(message, options);
    };

    window.confirmDelete = (itemName) => {
        console.log('📞 confirmDelete chamado:', itemName);
        return window.alertSystem.confirmDelete(itemName);
    };

    console.log('✅ Sistema de Alertas inicializado');

    // Debug: verificar se o CSS foi carregado
    const testElement = document.createElement('div');
    testElement.className = 'alert-modern';
    testElement.style.display = 'none';
    document.body.appendChild(testElement);

    const computedStyle = window.getComputedStyle(testElement);
    const hasStyles = computedStyle.borderRadius !== '0px';

    document.body.removeChild(testElement);

    if (hasStyles) {
        console.log('✅ CSS do Alert System carregado corretamente');
        console.log('📐 Estilos CSS encontrados:', {
            borderRadius: computedStyle.borderRadius,
            boxShadow: computedStyle.boxShadow,
            background: computedStyle.background
        });
    } else {
        console.warn('⚠️ CSS do Alert System pode não estar carregado. Verifique se alert-system.css está incluído no _Layout.cshtml');
    }

    // Função de teste rápido disponível no console
    window.testAlertSystem = function() {
        console.log('🧪 Executando teste do Alert System...');

        // Testar cada tipo de alerta
        setTimeout(() => window.showSuccess('Teste de sucesso!', 'Sucesso'), 100);
        setTimeout(() => window.showError('Teste de erro!', 'Erro'), 1500);
        setTimeout(() => window.showWarning('Teste de aviso!', 'Aviso'), 3000);
        setTimeout(() => window.showInfo('Teste de informação!', 'Info'), 4500);

        console.log('✅ Teste agendado. Observe os alertas no canto superior direito.');
    };

    console.log('💡 Para testar o sistema, execute: testAlertSystem()');
}
