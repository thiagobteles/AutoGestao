document.addEventListener('DOMContentLoaded', function () {
    initializeStandardForm();
});

function initializeStandardForm() {
    // Inicializar funcionalidades
    initializeMasks();
    initializeConditionalFields();
    initializeAjaxForm();
    initializeCepAutoFill();

    console.log('Sistema de formulários dinâmicos inicializado');
}

// ================================================================================================
// MÁSCARAS PARA CAMPOS BRASILEIROS
// ================================================================================================

function initializeMasks() {
    // Máscara para telefone
    document.querySelectorAll('.phone-mask').forEach(function (input) {
        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');

            if (value.length <= 10) {
                value = value.replace(/(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{4})(\d)/, '$1-$2');
            } else {
                value = value.replace(/(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{5})(\d)/, '$1-$2');
            }

            this.value = value;
        });
    });

    // Máscara para CPF
    document.querySelectorAll('.cpf-mask').forEach(function (input) {
        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d{1,2})/, '$1-$2');
            this.value = value;
        });
    });

    // Máscara para CNPJ
    document.querySelectorAll('.cnpj-mask').forEach(function (input) {
        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');
            value = value.replace(/(\d{2})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1.$2');
            value = value.replace(/(\d{3})(\d)/, '$1/$2');
            value = value.replace(/(\d{4})(\d{1,2})/, '$1-$2');
            this.value = value;
        });
    });

    // Máscara para CEP
    document.querySelectorAll('.cep-mask').forEach(function (input) {
        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');
            value = value.replace(/(\d{5})(\d)/, '$1-$2');
            this.value = value;
        });
    });

    // Máscara para Currency (APENAS para campos com tipo Currency)
    document.querySelectorAll('.currency-mask').forEach(function (input) {
        // Formatar valor inicial se já existir
        if (input.value && input.value.trim() !== '' && !input.value.includes('R$')) {
            const numValue = parseFloat(input.value.replace(',', '.'));
            if (!isNaN(numValue)) {
                input.value = formatToCurrency(numValue);
            }
        }

        input.addEventListener('input', function (e) {
            let value = this.value;

            // Remove tudo exceto dígitos e vírgula
            value = value.replace(/[^\d,]/g, '');

            // Remove vírgulas duplicadas
            const parts = value.split(',');
            if (parts.length > 2) {
                value = parts[0] + ',' + parts.slice(1).join('');
            }

            // Limita a 2 casas decimais após a vírgula
            if (parts.length === 2 && parts[1].length > 2) {
                value = parts[0] + ',' + parts[1].substring(0, 2);
            }

            // Aplica formatação de milhar
            if (value) {
                const [inteiro, decimal] = value.split(',');
                const inteiroFormatado = inteiro.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
                value = decimal !== undefined ? `${inteiroFormatado},${decimal}` : inteiroFormatado;
                this.value = 'R$ ' + value;
            } else {
                this.value = '';
            }
        });

        input.addEventListener('blur', function () {
            if (this.value && this.value !== 'R$ ') {
                let value = this.value.replace(/[^\d,]/g, '');
                if (value && !value.includes(',')) {
                    this.value = 'R$ ' + value + ',00';
                } else if (value && value.endsWith(',')) {
                    this.value = 'R$ ' + value + '00';
                } else if (value) {
                    const [inteiro, decimal] = value.split(',');
                    if (decimal && decimal.length === 1) {
                        const inteiroFormatado = inteiro.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
                        this.value = 'R$ ' + inteiroFormatado + ',' + decimal + '0';
                    }
                }
            }
        });
    });

    // Máscara para Percentage
    document.querySelectorAll('.percentage-mask').forEach(function (input) {
        formatPercentageValue(input);

        input.addEventListener('input', function (e) {
            let value = this.value.replace(/[^\d,]/g, '');
            const parts = value.split(',');
            if (parts.length > 2) {
                value = parts[0] + ',' + parts.slice(1).join('');
            }
            if (parts.length === 2 && parts[1].length > 2) {
                value = parts[0] + ',' + parts[1].substring(0, 2);
            }
            this.value = value ? value + '%' : '';
        });
    });
}

function formatToCurrency(value) {
    if (!value && value !== 0) return '';

    const formatted = value.toFixed(2)
        .replace('.', ',')
        .replace(/\B(?=(\d{3})+(?!\d))/g, '.');

    return 'R$ ' + formatted;
}

/**
 * Converte valor de moeda brasileira para decimal
 * R$ 89.000,00 -> 89000.00
 */
function parseCurrencyToDecimal(value) {
    if (!value || value.trim() === '') {
        return '';
    }

    // Remove R$, espaços e pontos (separador de milhar)
    let cleanValue = value
        .replace('R$', '')
        .replace(/\s/g, '')
        .replace(/\./g, ''); // Remove pontos de milhar

    // Substitui vírgula decimal por ponto
    cleanValue = cleanValue.replace(',', '.');

    // Remove qualquer caractere que não seja dígito ou ponto
    cleanValue = cleanValue.replace(/[^\d.]/g, '');

    // Valida o resultado
    const numericValue = parseFloat(cleanValue);

    if (isNaN(numericValue)) {
        return '0';
    }

    return numericValue.toString();
}

/**
 * Converte valor de porcentagem para decimal
 */
function parsePercentageToDecimal(value) {
    if (!value || value.trim() === '') {
        return '';
    }

    let cleanValue = value
        .replace(/%/g, '')
        .replace(/\s/g, '')
        .replace(',', '.');

    const numericValue = parseFloat(cleanValue);

    if (isNaN(numericValue)) {
        return '0';
    }

    return numericValue.toString();
}

function formatPercentageValue(input) {
    if (!input.value || input.value.trim() === '' || input.value === '%') {
        input.value = '';
        return;
    }

    if (input.value.endsWith('%') && !input.value.includes('%%')) {
        return;
    }

    let value = input.value.replace(/[^\d,]/g, '');

    if (value === '') {
        input.value = '';
        return;
    }

    if (!value.includes(',') && value.includes('.')) {
        value = value.replace('.', ',');
    }

    input.value = value + '%';
}


// ================================================================================================
// CAMPOS CONDICIONAIS
// ================================================================================================

function initializeConditionalFields() {
    document.querySelectorAll('[data-conditional-field]').forEach(function (field) {
        const conditionalField = field.dataset.conditionalField;
        const conditionalValue = field.dataset.conditionalValue;

        if (conditionalField) {
            const triggerField = document.querySelector(`[name="${conditionalField}"]`);

            if (triggerField) {
                function checkCondition() {
                    const triggerValue = triggerField.value;

                    if (triggerValue === conditionalValue) {
                        showField(field);
                    } else {
                        hideField(field);
                    }
                }

                triggerField.addEventListener('change', checkCondition);
                triggerField.addEventListener('input', checkCondition);

                // Verificar condição inicial
                checkCondition();
            }
        }
    });
}

function showField(field) {
    field.style.display = '';
    field.classList.remove('hiding');
    field.classList.add('showing');

    // Reativar validação se necessário
    const input = field.querySelector('input, select, textarea');
    if (input && input.hasAttribute('data-was-required')) {
        input.setAttribute('required', '');
        input.removeAttribute('data-was-required');
    }
}

function hideField(field) {
    field.classList.remove('showing');
    field.classList.add('hiding');

    setTimeout(() => {
        field.style.display = 'none';
        field.classList.remove('hiding');
    }, 300);

    // Desativar validação
    const input = field.querySelector('input, select, textarea');
    if (input && input.hasAttribute('required')) {
        input.setAttribute('data-was-required', '');
        input.removeAttribute('required');
        clearFieldErrors(input);
    }
}

// ================================================================================================
// SUBMISSÃO AJAX
// ================================================================================================

async function submitFormAjax(form) {
    const submitBtn = form.querySelector('#submitBtn');
    const originalText = submitBtn?.innerHTML;

    try {
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Salvando...';
        }

        clearAllErrors(form);

        const formData = new FormData(form);
        const actionUrl = form.action;

        const response = await fetch(actionUrl, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const result = await response.json();

        if (result.success || result.sucesso) {

            if (result.script) {
                try {
                    eval(result.script);
                } catch (scriptError) {
                    console.error('Erro ao executar script:', scriptError);
                }
            } else {
                showSuccess(result.message || result.mensagem || 'Salvo com sucesso!');
            }

            const redirectUrl = result.redirectUrl ||
                window.location.pathname.replace(/\/(Create|Edit).*/, '');

            if (window.responseHandler) {
                window.responseHandler.isNavigating = true;
            }

            setTimeout(() => {
                window.location.href = redirectUrl;
            }, 1500);

        } else {
            if (result.errors) {
                displayErrors(result.errors, form);
            }

            const errorMessage = result.message || result.mensagem || 'Erro ao salvar';

            if (result.script && result.script.includes('Error')) {
                eval(result.script);
            } else {
                showError(errorMessage);
            }

            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        }

    } catch (error) {
        console.error('[FORM] Erro:', error);

        const isNavigating = window.responseHandler?.isNavigating || false;
        const isAbortError = error.name === 'AbortError' ||
            error.message?.includes('aborted');

        if (!isNavigating && !isAbortError) {
            showError('Erro ao processar requisição');

            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        }
    }
}

function displayValidationErrors(form, errors) {
    for (const [fieldName, messages] of Object.entries(errors)) {
        const field = form.querySelector(`[name="${fieldName}"]`);

        if (field) {
            field.classList.add('is-invalid');

            const existingFeedback = field.parentElement.querySelector('.invalid-feedback');
            if (existingFeedback) {
                existingFeedback.remove();
            }

            const feedback = document.createElement('div');
            feedback.className = 'invalid-feedback';
            feedback.style.display = 'block';
            feedback.textContent = Array.isArray(messages) ? messages[0] : messages;

            field.parentElement.appendChild(feedback);
        }
    }
}

function displayErrors(errors, form) {
    if (typeof errors === 'object') {
        displayValidationErrors(form, errors);
    }
}

function clearAllErrors(form) {
    form.querySelectorAll('.is-invalid').forEach(field => {
        field.classList.remove('is-invalid');
    });

    form.querySelectorAll('.invalid-feedback').forEach(error => {
        error.remove();
    });

    const alertDanger = form.querySelector('.alert-danger');
    if (alertDanger) {
        alertDanger.style.display = 'none';
    }
}

function clearFieldErrors(field) {
    field.classList.remove('is-invalid');

    const errorElement = field.parentNode.querySelector('.invalid-feedback');
    if (errorElement) {
        errorElement.remove();
    }
}

function initializeAjaxForm() {
    const form = document.querySelector('.standard-form');

    if (!form || form.dataset.ajax !== 'true') return;

    // Prevenir múltiplas inicializações
    if (form.dataset.ajaxInitialized === 'true') {
        console.log('Formulário AJAX já inicializado, ignorando duplicação');
        return;
    }

    console.log('Inicializando formulário AJAX');
    form.dataset.ajaxInitialized = 'true';

    // Remover listeners anteriores se existirem
    const oldSubmitHandler = form._submitHandler;
    if (oldSubmitHandler) {
        form.removeEventListener('submit', oldSubmitHandler);
    }

    // Criar novo handler e armazenar referência
    const submitHandler = function (e) {
        e.preventDefault();
        e.stopImmediatePropagation(); // Prevenir outros handlers
        submitFormAjax(this);
    };

    form._submitHandler = submitHandler;
    form.addEventListener('submit', submitHandler);
}

(function () {
    window.addEventListener('beforeunload', function () {
        if (window.responseHandler) {
            window.responseHandler.isNavigating = true;
        }
    });
})();

// Remover esta inicialização duplicada - já é chamado via initializeStandardForm() no DOMContentLoaded
// if (document.readyState === 'loading') {
//     document.addEventListener('DOMContentLoaded', initializeAjaxForm);
// } else {
//     initializeAjaxForm();
// }

// ================================================================================================
// BUSCA AUTOMÁTICA DE CEP
// ================================================================================================

function initializeCepAutoFill() {
    document.querySelectorAll('[data-cep-auto-fill="true"]').forEach(function (cepField) {
        cepField.addEventListener('blur', function () {
            const cep = this.value.replace(/\D/g, '');

            if (cep.length === 8) {
                buscarCep(cep);
            }
        });
    });
}

async function buscarCep(cep) {
    try {
        showInfo('Buscando CEP...');

        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const data = await response.json();

        if (data.erro) {
            showWarning('CEP não encontrado!');
            return;
        }

        // Preencher campos automaticamente
        const mappings = {
            'Endereco': data.logradouro,
            'Bairro': data.bairro,
            'Cidade': data.localidade,
            'Estado': data.uf
        };

        Object.entries(mappings).forEach(([fieldName, value]) => {
            const field = document.querySelector(`[name="${fieldName}"]`);
            if (field && value) {
                field.value = value;
                field.dispatchEvent(new Event('change'));

                // Animação visual
                const successLighter = getComputedStyle(document.documentElement).getPropertyValue('--success-lighter').trim();
                field.style.backgroundColor = successLighter;
                setTimeout(() => {
                    field.style.backgroundColor = '';
                }, 1000);
            }
        });

        showSuccess('CEP encontrado com sucesso!');

    } catch (error) {
        console.error('Erro ao buscar CEP:', error);
        showError('Erro ao buscar CEP!');
    }
}

// ================================================================================================
// UTILITÁRIOS
// ================================================================================================

function getBootstrapClass(type) {
    const mapping = {
        'success': 'success',
        'error': 'danger',
        'warning': 'warning',
        'info': 'info'
    };
    return mapping[type] || 'info';
}

// ================================================================================================
// CSS ADICIONAL PARA ANIMAÇÕES
// ================================================================================================

// Adicionar estilo CSS dinamicamente
const style = document.createElement('style');
style.textContent = `
    .form-group-modern[data-conditional-field] {
        transition: all 0.3s ease-in-out;
    }
    
    .form-group-modern[data-conditional-field].hiding {
        opacity: 0;
        max-height: 0;
        margin: 0;
        padding: 0;
        overflow: hidden;
    }
    
    .form-group-modern[data-conditional-field].showing {
        opacity: 1;
        max-height: 200px;
        transition-delay: 0.1s;
    }
    
    .standard-form.loading {
        position: relative;
        pointer-events: none;
        opacity: 0.7;
    }
`;

document.head.appendChild(style);