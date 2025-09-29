/**
 * 🎯 CAMPOS CONDICIONAIS - Sistema dinâmico e robusto
 * Suporta: Enums, múltiplos valores, operadores especiais
 */

// ================================================================================================
// INICIALIZAÇÃO
// ================================================================================================

function initializeConditionalFields() {
    console.log('🔄 Inicializando campos condicionais...');

    const conditionalFields = document.querySelectorAll('[data-conditional-field]');

    if (conditionalFields.length === 0) {
        console.log('ℹ️ Nenhum campo condicional encontrado');
        return;
    }

    conditionalFields.forEach(field => {
        const conditionalFieldName = field.dataset.conditionalField;
        const conditionalValue = field.dataset.conditionalValue;

        if (!conditionalFieldName || !conditionalValue) {
            console.warn('⚠️ Campo condicional sem configuração completa:', field);
            return;
        }

        // Busca o elemento trigger (campo que controla a visibilidade)
        const triggerElement = findTriggerElement(conditionalFieldName);

        if (!triggerElement) {
            console.warn(`⚠️ Campo trigger não encontrado: ${conditionalFieldName}`);
            return;
        }

        console.log(`✅ Configurando condicional: ${field.dataset.targetField || field.id} depende de ${conditionalFieldName} = ${conditionalValue}`);

        // Função que verifica a condição
        const checkCondition = () => evaluateAndToggleField(field, triggerElement, conditionalValue);

        // Verificar condição inicial
        checkCondition();

        // Escutar mudanças no campo trigger
        triggerElement.addEventListener('change', checkCondition);
        triggerElement.addEventListener('input', checkCondition);
    });

    console.log(`✅ ${conditionalFields.length} campos condicionais configurados`);
}

// ================================================================================================
// BUSCA DE ELEMENTOS
// ================================================================================================

function findTriggerElement(fieldName) {
    // Tenta diferentes seletores
    const selectors = [
        `[name="${fieldName}"]`,
        `#${fieldName}`,
        `[data-field-name="${fieldName}"]`,
        `[id$="${fieldName}"]`
    ];

    for (const selector of selectors) {
        const element = document.querySelector(selector);
        if (element) {
            return element;
        }
    }

    return null;
}

// ================================================================================================
// AVALIAÇÃO DE CONDIÇÕES
// ================================================================================================

function evaluateAndToggleField(field, triggerElement, conditionalValue) {
    const currentValue = getTriggerValue(triggerElement);
    const shouldShow = evaluateCondition(currentValue, conditionalValue);

    if (shouldShow) {
        showField(field);
    } else {
        hideField(field);
    }
}

function getTriggerValue(element) {
    if (!element) return '';

    // Se for checkbox
    if (element.type === 'checkbox') {
        return element.checked ? 'true' : 'false';
    }

    // Se for radio
    if (element.type === 'radio') {
        const checked = document.querySelector(`[name="${element.name}"]:checked`);
        return checked ? checked.value : '';
    }

    // Valor padrão
    return element.value || '';
}

function evaluateCondition(currentValue, conditionalValue) {
    // Operador >0 (maior que zero / não vazio)
    if (conditionalValue === '>0') {
        return currentValue &&
            currentValue !== '0' &&
            currentValue !== '' &&
            currentValue !== 'false';
    }

    // Operador ! (diferente de)
    if (conditionalValue.startsWith('!')) {
        const notValue = conditionalValue.substring(1);
        return currentValue !== notValue;
    }

    // Múltiplos valores (OR logic) - ex: "1,2,3"
    if (conditionalValue.includes(',')) {
        const values = conditionalValue.split(',').map(v => v.trim());
        return values.includes(currentValue);
    }

    // Comparação direta (case-insensitive para strings)
    return currentValue.toString().toLowerCase() === conditionalValue.toString().toLowerCase();
}

// ================================================================================================
// MOSTRAR/ESCONDER CAMPOS
// ================================================================================================

function showField(field) {
    // Busca o container pai (col-md-X)
    const container = findFieldContainer(field);

    if (!container) {
        field.style.display = '';
        return;
    }

    // Remove classes de animação
    container.classList.remove('hiding');
    container.classList.add('showing');

    // Mostra o campo
    container.style.display = '';

    // Reativa validação se necessário
    const input = field.querySelector('input, select, textarea');
    if (input && input.hasAttribute('data-was-required')) {
        input.setAttribute('required', '');
        input.removeAttribute('data-was-required');
    }

    // Remove mensagens de erro antigas
    clearFieldErrors(input);
}

function hideField(field) {
    const container = findFieldContainer(field);

    if (!container) {
        field.style.display = 'none';
        return;
    }

    // Animação de saída
    container.classList.remove('showing');
    container.classList.add('hiding');

    setTimeout(() => {
        container.style.display = 'none';
        container.classList.remove('hiding');
    }, 300);

    // Desativa validação
    const input = field.querySelector('input, select, textarea');
    if (input) {
        if (input.hasAttribute('required')) {
            input.setAttribute('data-was-required', 'true');
            input.removeAttribute('required');
        }

        // Limpa o valor do campo escondido
        if (input.tagName === 'SELECT') {
            input.selectedIndex = 0;
        } else if (input.type !== 'checkbox' && input.type !== 'radio') {
            input.value = '';
        }

        clearFieldErrors(input);
    }
}

function findFieldContainer(field) {
    // Busca o container de coluna do Bootstrap
    return field.closest('.col-md-1, .col-md-2, .col-md-3, .col-md-4, .col-md-6, .col-md-12, .form-group');
}

function clearFieldErrors(input) {
    if (!input) return;

    input.classList.remove('is-invalid');

    const feedback = input.parentElement?.querySelector('.invalid-feedback');
    if (feedback) {
        feedback.style.display = 'none';
    }
}

// ================================================================================================
// AUTO-INICIALIZAÇÃO
// ================================================================================================

// Inicializa quando o DOM estiver pronto
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeConditionalFields);
} else {
    initializeConditionalFields();
}

// Re-inicializa em modais
document.addEventListener('shown.bs.modal', function () {
    setTimeout(initializeConditionalFields, 100);
});

// Expõe funções globalmente para re-inicialização manual
window.ConditionalFields = {
    initialize: initializeConditionalFields,
    evaluateCondition: evaluateCondition,
    showField: showField,
    hideField: hideField
};

console.log('✅ Módulo de campos condicionais carregado');