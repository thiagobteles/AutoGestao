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


/**
 * 🚀 CAMPOS CONDICIONAIS AVANÇADOS
 * Suporta regras complexas, múltiplas condições, funções especiais
 * Sistema compatível com expressões como:
 * - "TipoCliente == PessoaFisica AND Age(DataNascimento) >= 10"
 * - "Valor > 1000 OR Status == Aprovado"
 */

// ================================================================================================
// ENGINE DE AVALIAÇÃO DE EXPRESSÕES
// ================================================================================================

class ConditionalExpressionEngine {
    constructor() {
        this.formData = {};
    }

    /**
     * Avalia uma expressão condicional completa
     */
    evaluate(expression, formData = null) {
        if (!expression || expression.trim() === '') {
            return true;
        }

        this.formData = formData || this.collectFormData();

        try {
            // OR tem precedência menor que AND
            if (expression.toUpperCase().includes(' OR ')) {
                return this.evaluateOrExpression(expression);
            }

            if (expression.toUpperCase().includes(' AND ')) {
                return this.evaluateAndExpression(expression);
            }

            // Expressão simples
            return this.evaluateSimpleExpression(expression);
        } catch (error) {
            console.error(`Erro ao avaliar expressão "${expression}":`, error);
            return false;
        }
    }

    /**
     * Avalia expressão com operador OR
     */
    evaluateOrExpression(expression) {
        const parts = expression.split(/\s+OR\s+/i);

        for (const part of parts) {
            if (this.evaluate(part.trim())) {
                return true; // Qualquer TRUE = resultado TRUE
            }
        }

        return false;
    }

    /**
     * Avalia expressão com operador AND
     */
    evaluateAndExpression(expression) {
        const parts = expression.split(/\s+AND\s+/i);

        for (const part of parts) {
            if (!this.evaluate(part.trim())) {
                return false; // Qualquer FALSE = resultado FALSE
            }
        }

        return true;
    }

    /**
     * Avalia expressão simples
     */
    evaluateSimpleExpression(expression) {
        expression = expression.trim();

        // Verifica se é uma função
        if (expression.includes('(')) {
            return this.evaluateFunction(expression);
        }

        // Expressão de comparação: campo operador valor
        const match = expression.match(/^(\w+)\s*(==|!=|>|<|>=|<=)\s*(.+)$/);

        if (!match) {
            console.warn(`Expressão inválida: ${expression}`);
            return false;
        }

        const [, fieldName, operator, expectedValue] = match;
        const currentValue = this.getFieldValue(fieldName);

        return this.evaluateOperator(currentValue, operator, expectedValue.trim());
    }

    /**
     * Avalia funções especiais
     */
    evaluateFunction(expression) {
        // Age(campo) >= valor
        let match = expression.match(/Age\((\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)/i);
        if (match) {
            return this.evaluateAgeFunction(match);
        }

        // HasValue(campo)
        match = expression.match(/HasValue\((\w+)\)/i);
        if (match) {
            return this.evaluateHasValueFunction(match[1]);
        }

        // IsEmpty(campo)
        match = expression.match(/IsEmpty\((\w+)\)/i);
        if (match) {
            return !this.evaluateHasValueFunction(match[1]);
        }

        // Length(campo) > valor
        match = expression.match(/Length\((\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)/i);
        if (match) {
            return this.evaluateLengthFunction(match);
        }

        // DateDiff(campo1, campo2, unidade) > valor
        match = expression.match(/DateDiff\((\w+),\s*(\w+),\s*(\w+)\)\s*(==|!=|>|<|>=|<=)\s*(\d+)/i);
        if (match) {
            return this.evaluateDateDiffFunction(match);
        }

        console.warn(`Função não reconhecida: ${expression}`);
        return false;
    }

    /**
     * Avalia função Age() - Calcula idade baseado em data de nascimento
     */
    evaluateAgeFunction(match) {
        const [, fieldName, operator, expectedAge] = match;
        const dateValue = this.getFieldValue(fieldName);

        if (!dateValue) {
            return false;
        }

        const age = this.calculateAge(new Date(dateValue));
        return this.compareNumbers(age, operator, parseInt(expectedAge));
    }

    /**
     * Avalia função HasValue() - Verifica se campo tem valor
     */
    evaluateHasValueFunction(fieldName) {
        const value = this.getFieldValue(fieldName);

        if (value === null || value === undefined) {
            return false;
        }

        if (typeof value === 'string') {
            return value.trim() !== '';
        }

        if (typeof value === 'number') {
            return value !== 0;
        }

        return true;
    }

    /**
     * Avalia função Length() - Tamanho de string
     */
    evaluateLengthFunction(match) {
        const [, fieldName, operator, expectedLength] = match;
        const value = this.getFieldValue(fieldName);

        const length = value ? value.toString().length : 0;
        return this.compareNumbers(length, operator, parseInt(expectedLength));
    }

    /**
     * Avalia função DateDiff() - Diferença entre datas
     */
    evaluateDateDiffFunction(match) {
        const [, field1, field2, unit, operator, expectedDiff] = match;

        const date1 = new Date(this.getFieldValue(field1));
        const date2 = new Date(this.getFieldValue(field2));

        if (isNaN(date1.getTime()) || isNaN(date2.getTime())) {
            return false;
        }

        const diffMs = Math.abs(date1 - date2);
        const diffDays = diffMs / (1000 * 60 * 60 * 24);

        let actualDiff;
        switch (unit.toLowerCase()) {
            case 'days':
                actualDiff = Math.floor(diffDays);
                break;
            case 'months':
                actualDiff = Math.floor(diffDays / 30);
                break;
            case 'years':
                actualDiff = Math.floor(diffDays / 365);
                break;
            default:
                actualDiff = Math.floor(diffDays);
        }

        return this.compareNumbers(actualDiff, operator, parseInt(expectedDiff));
    }

    /**
     * Obtém valor de um campo do formulário
     */
    getFieldValue(fieldName) {
        // Tenta várias formas de encontrar o campo
        const selectors = [
            `[name="${fieldName}"]`,
            `#${fieldName}`,
            `[data-field-name="${fieldName}"]`
        ];

        for (const selector of selectors) {
            const element = document.querySelector(selector);

            if (element) {
                if (element.type === 'checkbox') {
                    return element.checked;
                }

                if (element.type === 'radio') {
                    const checked = document.querySelector(`[name="${fieldName}"]:checked`);
                    return checked ? checked.value : '';
                }

                return element.value;
            }
        }

        // Fallback para dados coletados
        return this.formData[fieldName] || '';
    }

    /**
     * Avalia operador de comparação
     */
    evaluateOperator(currentValue, operator, expectedValue) {
        // Normaliza valores
        currentValue = this.normalizeValue(currentValue);
        expectedValue = this.normalizeValue(expectedValue);

        switch (operator) {
            case '==':
                return currentValue.toLowerCase() === expectedValue.toLowerCase();

            case '!=':
                return currentValue.toLowerCase() !== expectedValue.toLowerCase();

            case '>':
            case '<':
            case '>=':
            case '<=':
                return this.compareNumericValues(currentValue, operator, expectedValue);

            default:
                return false;
        }
    }

    /**
     * Compara valores numéricos
     */
    compareNumericValues(value1, operator, value2) {
        const num1 = parseFloat(value1);
        const num2 = parseFloat(value2);

        if (isNaN(num1) || isNaN(num2)) {
            return false;
        }

        return this.compareNumbers(num1, operator, num2);
    }

    /**
     * Compara números com operador
     */
    compareNumbers(num1, operator, num2) {
        switch (operator) {
            case '==': return num1 === num2;
            case '!=': return num1 !== num2;
            case '>': return num1 > num2;
            case '<': return num1 < num2;
            case '>=': return num1 >= num2;
            case '<=': return num1 <= num2;
            default: return false;
        }
    }

    /**
     * Normaliza valor para comparação
     */
    normalizeValue(value) {
        if (value === null || value === undefined) {
            return '';
        }

        if (typeof value === 'boolean') {
            return value ? 'true' : 'false';
        }

        return value.toString().trim();
    }

    /**
     * Calcula idade baseado em data de nascimento
     */
    calculateAge(birthDate) {
        const today = new Date();
        let age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();

        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }

        return age;
    }

    /**
     * Coleta todos os dados do formulário
     */
    collectFormData() {
        const data = {};
        const form = document.querySelector('form');

        if (form) {
            const formData = new FormData(form);
            for (const [key, value] of formData.entries()) {
                data[key] = value;
            }
        }

        return data;
    }
}

// ================================================================================================
// INICIALIZAÇÃO DE CAMPOS CONDICIONAIS
// ================================================================================================

const evaluator = new ConditionalExpressionEngine();

function initializeAdvancedConditionalFields() {
    console.log('🚀 Inicializando campos condicionais avançados...');

    const conditionalFields = document.querySelectorAll('[data-conditional-display-rule], [data-conditional-field]');

    if (conditionalFields.length === 0) {
        console.log('ℹ️ Nenhum campo condicional encontrado');
        return;
    }

    conditionalFields.forEach(field => {
        setupConditionalField(field);
    });

    console.log(`✅ ${conditionalFields.length} campos condicionais configurados`);
}

function setupConditionalField(field) {
    // Suporta tanto o novo sistema (data-conditional-display-rule) quanto o antigo
    const displayRule = field.dataset.conditionalDisplayRule || buildLegacyRule(field);
    const requiredRule = field.dataset.conditionalRequiredRule || '';

    if (!displayRule && !requiredRule) {
        return;
    }

    console.log(`⚙️ Configurando campo: ${field.dataset.fieldName || field.id}`);
    console.log(`   Display Rule: ${displayRule}`);
    console.log(`   Required Rule: ${requiredRule}`);

    // Identifica campos que afetam esta regra
    const dependentFields = extractFieldNames(displayRule + ' ' + requiredRule);

    // Função de verificação
    const checkConditions = () => {
        // Verifica visibilidade
        if (displayRule) {
            const shouldDisplay = evaluator.evaluate(displayRule);
            toggleFieldVisibility(field, shouldDisplay);
        }

        // Verifica obrigatoriedade
        if (requiredRule) {
            const shouldBeRequired = evaluator.evaluate(requiredRule);
            toggleFieldRequired(field, shouldBeRequired);
        }
    };

    // Verifica inicialmente
    checkConditions();

    // Monitora mudanças nos campos dependentes
    dependentFields.forEach(fieldName => {
        const element = findFieldElement(fieldName);
        if (element) {
            element.addEventListener('change', checkConditions);
            element.addEventListener('input', checkConditions);
        }
    });
}

/**
 * Constrói regra no formato antigo (compatibilidade)
 */
function buildLegacyRule(field) {
    const conditionalField = field.dataset.conditionalField;
    const conditionalValue = field.dataset.conditionalValue;

    if (!conditionalField || !conditionalValue) {
        return '';
    }

    // Converte para o novo formato
    return `${conditionalField} == ${conditionalValue}`;
}

/**
 * Extrai nomes de campos de uma expressão
 */
function extractFieldNames(expression) {
    const fields = new Set();

    // Regex para encontrar nomes de campos
    const fieldPattern = /(\w+)\s*(==|!=|>|<|>=|<=)/g;
    let match;

    while ((match = fieldPattern.exec(expression)) !== null) {
        fields.add(match[1]);
    }

    // Também procura por campos dentro de funções
    const functionPattern = /\((\w+)(?:,\s*(\w+))?\)/g;
    while ((match = functionPattern.exec(expression)) !== null) {
        fields.add(match[1]);
        if (match[2]) fields.add(match[2]);
    }

    return Array.from(fields);
}

/**
 * Encontra elemento de um campo
 */
function findFieldElement(fieldName) {
    const selectors = [
        `[name="${fieldName}"]`,
        `#${fieldName}`,
        `[data-field-name="${fieldName}"]`
    ];

    for (const selector of selectors) {
        const element = document.querySelector(selector);
        if (element) return element;
    }

    return null;
}

/**
 * Alterna visibilidade do campo
 */
function toggleFieldVisibility(field, shouldShow) {
    const container = field.closest('.col-md-1, .col-md-2, .col-md-3, .col-md-4, .col-md-6, .col-md-12, .form-group');
    const target = container || field;

    if (shouldShow) {
        target.style.display = '';
        target.classList.remove('hiding');
        target.classList.add('showing');

        // Reativa validação
        const input = field.querySelector('input, select, textarea');
        if (input && input.hasAttribute('data-was-required')) {
            input.setAttribute('required', '');
            input.removeAttribute('data-was-required');
        }
    } else {
        target.classList.remove('showing');
        target.classList.add('hiding');

        setTimeout(() => {
            target.style.display = 'none';
            target.classList.remove('hiding');
        }, 300);

        // Desativa validação e limpa valor
        const input = field.querySelector('input, select, textarea');
        if (input) {
            if (input.hasAttribute('required')) {
                input.setAttribute('data-was-required', 'true');
                input.removeAttribute('required');
            }

            // Limpa valor
            if (input.tagName === 'SELECT') {
                input.selectedIndex = 0;
            } else if (input.type !== 'checkbox' && input.type !== 'radio') {
                input.value = '';
            }

            input.classList.remove('is-invalid');
        }
    }
}

/**
 * Alterna obrigatoriedade do campo
 */
function toggleFieldRequired(field, shouldBeRequired) {
    const input = field.querySelector('input, select, textarea');

    if (!input) return;

    if (shouldBeRequired) {
        input.setAttribute('required', '');

        // Adiciona indicador visual
        const label = field.querySelector('label');
        if (label && !label.querySelector('.text-danger')) {
            label.innerHTML += ' <span class="text-danger">*</span>';
        }
    } else {
        input.removeAttribute('required');

        // Remove indicador visual
        const label = field.querySelector('label');
        const asterisk = label?.querySelector('.text-danger');
        if (asterisk) {
            asterisk.remove();
        }
    }
}

// ================================================================================================
// AUTO-INICIALIZAÇÃO
// ================================================================================================

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeAdvancedConditionalFields);
} else {
    initializeAdvancedConditionalFields();
}

// Re-inicializa em modais
document.addEventListener('shown.bs.modal', () => {
    setTimeout(initializeAdvancedConditionalFields, 100);
});

// Expõe API global
window.AdvancedConditionalFields = {
    initialize: initializeAdvancedConditionalFields,
    evaluator: evaluator,
    evaluate: (expression) => evaluator.evaluate(expression)
};

console.log('✅ Módulo de campos condicionais avançados carregado');