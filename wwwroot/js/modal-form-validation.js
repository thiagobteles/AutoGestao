/**
 * VALIDAÇÃO DE FORMULÁRIOS MODAIS
 * Sistema de validação client-side para formulários em modais
 *
 * Funcionalidades:
 * - Validação de campos obrigatórios antes do submit
 * - Popup com lista de erros detalhados
 * - Scroll automático até o primeiro campo com erro
 * - Destaque visual nos campos inválidos
 */

(function () {
    'use strict';

    /**
     * Inicializa a validação para todos os formulários modais
     */
    function initModalFormValidation() {
        console.log('🔍 Inicializando validação de formulários modais...');

        // Encontrar todos os formulários em modais
        const modalForms = document.querySelectorAll('.modal form');

        modalForms.forEach(form => {
            // Adicionar listener de submit
            form.addEventListener('submit', function (e) {
                const modal = form.closest('.modal');
                if (!modal) return;

                // Se for um modal de cadastro/edição, validar
                if (modal.id.includes('referenceCreateModal') || form.classList.contains('modal-form-container')) {
                    console.log('🔍 Validando formulário modal antes do submit...');

                    const validationResult = validateForm(form, modal);

                    if (!validationResult.isValid) {
                        e.preventDefault();
                        e.stopPropagation();

                        // Mostrar popup com erros
                        showValidationErrors(validationResult.errors, modal);

                        // Scroll até o primeiro campo com erro
                        scrollToFirstError(validationResult.errors, modal);

                        console.log('❌ Validação falhou:', validationResult.errors);
                    } else {
                        console.log('✅ Validação passou');
                    }
                }
            });
        });
    }

    /**
     * Valida todos os campos obrigatórios do formulário
     * @param {HTMLFormElement} form - Formulário a validar
     * @param {HTMLElement} modal - Modal container
     * @returns {Object} Resultado da validação
     */
    function validateForm(form, modal) {
        const errors = [];

        // Encontrar todos os campos obrigatórios
        const requiredFields = form.querySelectorAll('[required]');

        requiredFields.forEach(field => {
            // Verificar se o campo está visível (pode estar oculto por ConditionalDisplay)
            const fieldContainer = field.closest('.form-group');
            if (!fieldContainer) return;

            const isVisible = fieldContainer.style.display !== 'none' &&
                              !fieldContainer.classList.contains('d-none');

            if (!isVisible) return; // Ignorar campos ocultos

            // Validar conforme o tipo de campo
            const fieldName = field.name || field.id;
            const label = getFieldLabel(field);
            let isEmpty = false;

            if (field.type === 'checkbox') {
                isEmpty = !field.checked;
            } else if (field.type === 'file') {
                // Para campos file, verificar se já existe arquivo ou se novo foi selecionado
                const existingFile = form.querySelector(`[name="${fieldName}_Existing"]`);
                isEmpty = !field.files.length && (!existingFile || !existingFile.value);
            } else if (field.tagName === 'SELECT') {
                isEmpty = !field.value || field.value === '';
            } else {
                isEmpty = !field.value || field.value.trim() === '';
            }

            if (isEmpty) {
                errors.push({
                    field: field,
                    fieldName: fieldName,
                    label: label,
                    message: `${label} é obrigatório`
                });

                // Adicionar classe de erro visual
                field.classList.add('is-invalid');
                if (fieldContainer) {
                    fieldContainer.classList.add('has-error');
                }
            } else {
                // Remover classe de erro se estava presente
                field.classList.remove('is-invalid');
                if (fieldContainer) {
                    fieldContainer.classList.remove('has-error');
                }
            }
        });

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    }

    /**
     * Obtém o label/nome amigável do campo
     * @param {HTMLElement} field - Campo input
     * @returns {string} Label do campo
     */
    function getFieldLabel(field) {
        const fieldContainer = field.closest('.form-group');
        if (!fieldContainer) return field.name || field.id || 'Campo';

        const label = fieldContainer.querySelector('label');
        if (label) {
            // Remover asterisco vermelho do texto
            return label.textContent.replace('*', '').trim();
        }

        return field.name || field.id || 'Campo';
    }

    /**
     * Mostra popup com lista de erros de validação
     * @param {Array} errors - Lista de erros
     * @param {HTMLElement} modal - Modal container
     */
    function showValidationErrors(errors, modal) {
        // Construir lista de erros em HTML
        const errorList = errors.map((error, index) =>
            `<li class="validation-error-item">
                <i class="fas fa-exclamation-circle me-2"></i>
                <strong>${error.label}</strong>
            </li>`
        ).join('');

        const errorMessage = `
            <div class="validation-errors-container">
                <p class="mb-3">Os seguintes campos obrigatórios não foram preenchidos:</p>
                <ul class="validation-error-list">
                    ${errorList}
                </ul>
                <p class="mt-3 text-muted small">Por favor, preencha todos os campos obrigatórios e tente novamente.</p>
            </div>
        `;

        // Usar o sistema de notificação existente
        if (window.showError) {
            window.showError(errorMessage);
        } else {
            alert(`Campos obrigatórios não preenchidos:\n\n${errors.map(e => '• ' + e.label).join('\n')}`);
        }

        // Adicionar estilos CSS inline se não existirem
        if (!document.getElementById('modal-validation-styles')) {
            const style = document.createElement('style');
            style.id = 'modal-validation-styles';
            style.textContent = `
                .validation-errors-container {
                    text-align: left;
                }

                .validation-error-list {
                    list-style: none;
                    padding: 0;
                    margin: 0;
                    max-height: 300px;
                    overflow-y: auto;
                }

                .validation-error-item {
                    padding: 8px 12px;
                    margin-bottom: 6px;
                    background: #fff5f5;
                    border-left: 3px solid #dc3545;
                    border-radius: 4px;
                    color: #721c24;
                    font-size: 0.9rem;
                }

                .validation-error-item i {
                    color: #dc3545;
                }

                .has-error {
                    animation: shake 0.4s ease-in-out;
                }

                @keyframes shake {
                    0%, 100% { transform: translateX(0); }
                    25% { transform: translateX(-5px); }
                    75% { transform: translateX(5px); }
                }

                .is-invalid {
                    border-color: #dc3545 !important;
                    box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25) !important;
                }
            `;
            document.head.appendChild(style);
        }
    }

    /**
     * Faz scroll até o primeiro campo com erro
     * @param {Array} errors - Lista de erros
     * @param {HTMLElement} modal - Modal container
     */
    function scrollToFirstError(errors, modal) {
        if (errors.length === 0) return;

        const firstErrorField = errors[0].field;
        const fieldContainer = firstErrorField.closest('.form-group');

        if (fieldContainer) {
            // Scroll suave até o campo com erro
            fieldContainer.scrollIntoView({
                behavior: 'smooth',
                block: 'center',
                inline: 'nearest'
            });

            // Focar no campo após o scroll (com pequeno delay)
            setTimeout(() => {
                firstErrorField.focus();
            }, 500);
        }
    }

    /**
     * Remove validação visual quando o usuário começar a digitar
     */
    function setupLiveValidation() {
        document.addEventListener('input', function (e) {
            const field = e.target;

            if (field.classList.contains('is-invalid')) {
                // Verificar se agora tem valor
                let hasValue = false;

                if (field.type === 'checkbox') {
                    hasValue = field.checked;
                } else if (field.type === 'file') {
                    hasValue = field.files.length > 0;
                } else {
                    hasValue = field.value && field.value.trim() !== '';
                }

                if (hasValue) {
                    field.classList.remove('is-invalid');
                    const fieldContainer = field.closest('.form-group');
                    if (fieldContainer) {
                        fieldContainer.classList.remove('has-error');
                    }
                }
            }
        });
    }

    // Inicializar quando o DOM estiver pronto
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initModalFormValidation();
            setupLiveValidation();
        });
    } else {
        initModalFormValidation();
        setupLiveValidation();
    }

    // Reinicializar quando novos modais forem adicionados ao DOM
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            mutation.addedNodes.forEach(function (node) {
                if (node.nodeType === 1 && node.classList && node.classList.contains('modal')) {
                    console.log('🔄 Novo modal detectado, reinicializando validação...');
                    initModalFormValidation();
                }
            });
        });
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

    console.log('✅ Sistema de validação de formulários modais carregado');
})();
