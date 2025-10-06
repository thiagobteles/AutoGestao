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

function initializeAjaxForm() {
    const form = document.querySelector('.standard-form');

    if (!form || form.dataset.ajax !== 'true') return;

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        submitFormAjax(this);
    });
}

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

        // ===================================================================
        // CRITICAL: Garantir que campos de arquivo hidden sejam enviados
        // ===================================================================
        const filePathInputs = form.querySelectorAll('.file-path-input');
        filePathInputs.forEach(hiddenInput => {
            const fieldName = hiddenInput.name;
            const fieldValue = hiddenInput.value || '';

            console.log(`[FILE] ${fieldName} = ${fieldValue}`);

            // Garantir que o campo está no FormData
            if (formData.has(fieldName)) {
                formData.set(fieldName, fieldValue);
            } else {
                formData.append(fieldName, fieldValue);
            }
        });

        // Atualizar valores dos campos de referência
        const referenceHiddenInputs = form.querySelectorAll('input[type="hidden"][id$="_value"]');
        referenceHiddenInputs.forEach(hiddenInput => {
            const fieldName = hiddenInput.name;
            const fieldValue = hiddenInput.value || '0';

            console.log(`[REF] ${fieldName} = ${fieldValue}`);

            if (formData.has(fieldName)) {
                formData.set(fieldName, fieldValue);
            } else {
                formData.append(fieldName, fieldValue);
            }
        });

        const actionUrl = form.action;

        const response = await fetch(actionUrl, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        const result = await response.json();

        if (result.success) {
            showToast(result.message || 'Salvo com sucesso!', 'success');

            setTimeout(() => {
                if (result.redirectUrl) {
                    window.location.href = result.redirectUrl;
                } else {
                    window.location.href = form.dataset.backUrl || '/';
                }
            }, 1000);
        } else {
            if (result.errors) {
                displayValidationErrors(result.errors);
            } else {
                showToast(result.message || 'Erro ao salvar', 'error');
            }

            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalText;
            }
        }

    } catch (error) {
        console.error('Erro no submit:', error);
        showToast('Erro ao processar requisição', 'error');

        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
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
            feedback.textContent = Array.isArray(messages) ? messages.join(', ') : messages;
            field.parentElement.appendChild(feedback);
        }
    }
}

function clearAllErrors(form) {
    form.querySelectorAll('.is-invalid').forEach(el => {
        el.classList.remove('is-invalid');
    });

    form.querySelectorAll('.invalid-feedback').forEach(el => {
        el.remove();
    });

    const alertDanger = form.querySelector('.alert-danger');
    if (alertDanger) {
        alertDanger.style.display = 'none';
    }
}

function showToast(message, type = 'info') {
    if (window.showNotification) {
        window.showNotification(message, type);
        return;
    }

    const toast = document.createElement('div');
    toast.className = `alert alert-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'info'} position-fixed`;
    toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    toast.innerHTML = `
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        ${message}
    `;

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3000);
}

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
        showToast('Buscando CEP...', 'info');

        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const data = await response.json();

        if (data.erro) {
            showToast('CEP não encontrado!', 'warning');
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
                field.style.backgroundColor = '#d4edda';
                setTimeout(() => {
                    field.style.backgroundColor = '';
                }, 1000);
            }
        });

        showToast('CEP encontrado com sucesso!', 'success');

    } catch (error) {
        console.error('Erro ao buscar CEP:', error);
        showToast('Erro ao buscar CEP!', 'error');
    }
}

// ================================================================================================
// UTILITÁRIOS
// ================================================================================================

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

function showToast(message, type = 'info') {
    // Usar sistema de toast existente ou criar um simples
    if (window.showToast) {
        window.showToast(message, type);
    } else {
        // Toast simples se não houver sistema
        console.log(`${type.toUpperCase()}: ${message}`);

        // Criar toast simples
        const toast = document.createElement('div');
        toast.className = `alert alert-${getBootstrapClass(type)} toast-simple`;
        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            opacity: 0;
            transition: all 0.3s ease;
        `;
        toast.innerHTML = `
            <i class="${getToastIcon(type)}"></i>
            ${message}
            <button type="button" class="btn-close" onclick="this.parentNode.remove()"></button>
        `;

        document.body.appendChild(toast);

        // Mostrar toast
        setTimeout(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateY(0)';
        }, 100);

        // Remover automaticamente
        setTimeout(() => {
            toast.style.opacity = '0';
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    }
}

function getBootstrapClass(type) {
    const mapping = {
        'success': 'success',
        'error': 'danger',
        'warning': 'warning',
        'info': 'info'
    };
    return mapping[type] || 'info';
}

function getToastIcon(type) {
    const mapping = {
        'success': 'fas fa-check-circle',
        'error': 'fas fa-exclamation-circle',
        'warning': 'fas fa-exclamation-triangle',
        'info': 'fas fa-info-circle'
    };
    return mapping[type] || 'fas fa-info-circle';
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
    
    .toast-simple {
        transform: translateY(-20px);
    }
    
    .standard-form.loading {
        position: relative;
        pointer-events: none;
        opacity: 0.7;
    }
`;
document.head.appendChild(style);