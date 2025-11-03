// Sobrescrever mensagens de validação do jQuery Validate para português
(function () {
    if ($.validator) {
        $.extend($.validator.messages, {
            required: "Este campo é obrigatório.",
            remote: "Por favor, corrija este campo.",
            email: "Por favor, insira um endereço de e-mail válido.",
            url: "Por favor, insira uma URL válida.",
            date: "Por favor, insira uma data válida.",
            dateISO: "Por favor, insira uma data válida (ISO).",
            number: "Por favor, insira um número válido.",
            digits: "Por favor, insira apenas dígitos.",
            creditcard: "Por favor, insira um número de cartão de crédito válido.",
            equalTo: "Por favor, insira o mesmo valor novamente.",
            extension: "Por favor, insira um valor com uma extensão válida.",
            maxlength: $.validator.format("Por favor, insira no máximo {0} caracteres."),
            minlength: $.validator.format("Por favor, insira pelo menos {0} caracteres."),
            rangelength: $.validator.format("Por favor, insira um valor entre {0} e {1} caracteres."),
            range: $.validator.format("Por favor, insira um valor entre {0} e {1}."),
            max: $.validator.format("Por favor, insira um valor menor ou igual a {0}."),
            min: $.validator.format("Por favor, insira um valor maior ou igual a {0}."),
            step: $.validator.format("Por favor, insira um múltiplo de {0}.")
        });
    }

    // Sobrescrever mensagens de validação HTML5
    document.addEventListener('invalid', (function () {
        return function (e) {
            e.preventDefault();

            const input = e.target;
            const validity = input.validity;

            let message = '';

            if (validity.valueMissing) {
                const fieldName = input.getAttribute('data-field-name') ||
                    input.closest('.form-group-modern')?.querySelector('label')?.textContent?.trim()?.replace('*', '').trim() ||
                    input.name;
                message = `O campo ${fieldName} é obrigatório.`;
            } else if (validity.typeMismatch) {
                if (input.type === 'email') {
                    message = 'Por favor, insira um endereço de e-mail válido.';
                } else if (input.type === 'url') {
                    message = 'Por favor, insira uma URL válida.';
                }
            } else if (validity.patternMismatch) {
                message = input.getAttribute('title') || 'Por favor, use o formato correto.';
            } else if (validity.tooShort) {
                message = `Por favor, insira pelo menos ${input.minLength} caracteres.`;
            } else if (validity.tooLong) {
                message = `Por favor, insira no máximo ${input.maxLength} caracteres.`;
            } else if (validity.rangeUnderflow) {
                message = `Por favor, insira um valor maior ou igual a ${input.min}.`;
            } else if (validity.rangeOverflow) {
                message = `Por favor, insira um valor menor ou igual a ${input.max}.`;
            } else if (validity.stepMismatch) {
                message = `Por favor, insira um valor válido.`;
            }

            if (message) {
                input.setCustomValidity(message);

                // Adicionar classe de erro
                input.classList.add('is-invalid');

                // Criar ou atualizar mensagem de erro
                // Para campos de referência, colocar no container específico
                let container = null;
                if (input.classList.contains('reference-search-input')) {
                    const referenceContainer = input.closest('.reference-field-container');
                    if (referenceContainer) {
                        container = referenceContainer.querySelector('.reference-validation-container');
                    }
                }

                if (container) {
                    // Limpar feedback anterior
                    const existingFeedback = container.querySelector('.invalid-feedback');
                    if (existingFeedback) {
                        existingFeedback.remove();
                    }

                    // Adicionar novo feedback
                    const errorDiv = document.createElement('div');
                    errorDiv.className = 'invalid-feedback';
                    errorDiv.style.display = 'block';
                    errorDiv.textContent = message;
                    container.appendChild(errorDiv);
                } else {
                    // Para campos normais, manter comportamento padrão
                    let errorDiv = input.nextElementSibling;
                    if (!errorDiv || !errorDiv.classList.contains('invalid-feedback')) {
                        errorDiv = document.createElement('div');
                        errorDiv.className = 'invalid-feedback';
                        errorDiv.style.display = 'block';
                        input.parentNode.insertBefore(errorDiv, input.nextSibling);
                    }
                    errorDiv.textContent = message;
                    errorDiv.style.display = 'block';
                }
            }
        };
    })(), true);

    // Limpar mensagens de erro customizadas quando o campo for alterado
    document.addEventListener('input', function (e) {
        const input = e.target;
        if (input.validity.valid) {
            input.setCustomValidity('');
            input.classList.remove('is-invalid');

            // Para campos de referência, limpar no container específico
            if (input.classList.contains('reference-search-input')) {
                const referenceContainer = input.closest('.reference-field-container');
                if (referenceContainer) {
                    const container = referenceContainer.querySelector('.reference-validation-container');
                    if (container) {
                        const errorDiv = container.querySelector('.invalid-feedback');
                        if (errorDiv) {
                            errorDiv.remove();
                        }
                    }
                }
            } else {
                // Para campos normais
                const errorDiv = input.nextElementSibling;
                if (errorDiv && errorDiv.classList.contains('invalid-feedback')) {
                    errorDiv.style.display = 'none';
                }
            }
        }
    });

    // Limpar mensagens customizadas antes de validar novamente
    document.addEventListener('change', function (e) {
        const input = e.target;
        input.setCustomValidity('');
    });

})();

console.log('Validação em português carregada com sucesso');