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
                // Telefone fixo: (11) 1234-5678
                value = value.replace(/(\d{2})(\d)/, '($1) $2');
                value = value.replace(/(\d{4})(\d)/, '$1-$2');
            } else {
                // Celular: (11) 91234-5678
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

    // Máscara para Porcentagem
    document.querySelectorAll('.percentage-mask').forEach(function (input) {
        // Formatar valor inicial se já existir
        formatPercentageValue(input);

        input.addEventListener('input', function (e) {
            // Pegar posição do cursor
            let cursorPosition = this.selectionStart;
            let oldLength = this.value.length;

            // Remover tudo exceto números e vírgula
            let value = this.value.replace(/[^\d,]/g, '');

            // Permitir apenas uma vírgula
            const parts = value.split(',');
            if (parts.length > 2) {
                value = parts[0] + ',' + parts.slice(1).join('');
            }

            // Limitar casas decimais a 2
            if (parts.length === 2 && parts[1].length > 2) {
                value = parts[0] + ',' + parts[1].substring(0, 2);
            }

            // Adicionar % no final
            if (value !== '') {
                this.value = value + '%';
            } else {
                this.value = '';
            }

            // Ajustar posição do cursor
            let newLength = this.value.length;
            let diff = newLength - oldLength;

            // Se o cursor estava no final (onde está o %), manter antes do %
            if (cursorPosition === oldLength) {
                this.setSelectionRange(newLength - 1, newLength - 1);
            } else {
                this.setSelectionRange(cursorPosition + diff, cursorPosition + diff);
            }
        });

        input.addEventListener('focus', function () {
            // Remover % ao focar para facilitar edição
            this.value = this.value.replace('%', '');

            // Colocar cursor no final
            setTimeout(() => {
                this.setSelectionRange(this.value.length, this.value.length);
            }, 10);
        });

        input.addEventListener('blur', function () {
            formatPercentageValue(this);
        });

        // Prevenir que o usuário delete o % manualmente
        input.addEventListener('keydown', function (e) {
            // Se está tentando deletar e o cursor está no final (antes do %)
            if ((e.key === 'Backspace' || e.key === 'Delete') &&
                this.value.endsWith('%') &&
                this.selectionStart === this.value.length - 1) {
                // Permitir deletar o número, não o %
                e.preventDefault();
                let newValue = this.value.substring(0, this.value.length - 2) + '%';
                this.value = newValue === '%' ? '' : newValue;
                this.setSelectionRange(this.value.length - 1, this.value.length - 1);
            }
        });
    });

    // Máscara para moeda
    document.querySelectorAll('.currency-mask').forEach(function (input) {
        // IMPORTANTE: Formatar valor inicial se já existir
        formatCurrencyValue(input);

        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');

            if (value === '') {
                this.value = '';
                return;
            }

            // Converter para decimal
            value = (parseInt(value) / 100).toFixed(2);

            // Formatar como moeda brasileira
            value = value.replace('.', ',');
            value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');

            this.value = 'R$ ' + value;
        });

        // Remover formato ao focar para facilitar edição
        input.addEventListener('focus', function () {
            let value = this.value.replace(/[^\d,]/g, '');
            if (value) {
                this.value = value.replace(',', '.');
            }
        });

        // Aplicar formato ao sair do campo
        input.addEventListener('blur', function () {
            formatCurrencyValue(this);
        });
    });
}

// Função auxiliar para formatar valores de porcentagem
function formatPercentageValue(input) {
    if (!input.value || input.value.trim() === '' || input.value === '%') {
        input.value = '';
        return;
    }

    // Se já está formatado corretamente, não fazer nada
    if (input.value.endsWith('%') && !input.value.includes('%%')) {
        return;
    }

    // Remover % e espaços
    let value = input.value.replace(/[^\d,]/g, '');

    if (value === '') {
        input.value = '';
        return;
    }

    // Garantir formato correto com vírgula
    if (!value.includes(',') && value.includes('.')) {
        value = value.replace('.', ',');
    }

    // Adicionar %
    input.value = value + '%';
}

// Função auxiliar para formatar valores currency
function formatCurrencyValue(input) {
    if (!input.value || input.value.trim() === '') {
        return;
    }

    // Se já está formatado, não fazer nada
    if (input.value.startsWith('R$')) {
        return;
    }

    // Tentar parsear o valor
    let value = input.value.replace(/[^\d,.]/g, '').replace(',', '.');
    let numericValue = parseFloat(value);

    if (!isNaN(numericValue)) {
        // Formatar como moeda brasileira
        let formatted = numericValue.toFixed(2);
        formatted = formatted.replace('.', ',');
        formatted = formatted.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
        input.value = 'R$ ' + formatted;
    }
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
        // Mostrar loading
        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Salvando...';
        }

        // Limpar erros anteriores
        clearAllErrors(form);

        // Enviar formulário
        const formData = new FormData(form);
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                showToast(result.message || 'Formulário enviado com sucesso!', 'success');

                if (result.redirectUrl) {
                    setTimeout(() => {
                        window.location.href = result.redirectUrl;
                    }, 1500);
                }
            } else {
                showToast('Erro ao processar solicitação', 'error');
            }
        } else if (response.status === 400) {
            // Erros de validação
            const html = await response.text();
            const formContent = form.closest('.dashboard-container');

            if (formContent) {
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = html;
                const newFormContent = tempDiv.querySelector('form');

                if (newFormContent) {
                    form.parentNode.replaceChild(newFormContent, form);
                    initializeStandardForm(); // Reinicializar
                }
            }
        } else {
            showToast('Erro interno do servidor', 'error');
        }

    } catch (error) {
        console.error('Erro ao enviar formulário:', error);
        showToast('Erro ao enviar formulário', 'error');
    } finally {
        // Restaurar botão
        if (submitBtn && originalText) {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        }
    }
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