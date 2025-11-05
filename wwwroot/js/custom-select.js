/**
 * Custom Select com suporte a ícones Font Awesome
 * Converte selects com classe 'enum-select' em custom selects
 */

document.addEventListener('DOMContentLoaded', function() {
    initializeCustomSelects();
});

function initializeCustomSelects() {
    // Selecionar todos os selects que devem ser customizados
    const enumSelects = document.querySelectorAll('select.enum-select');

    enumSelects.forEach(select => {
        convertToCustomSelect(select);
    });
}

function convertToCustomSelect(selectElement) {
    // Verificar se já foi convertido
    if (selectElement.dataset.customized === 'true') {
        return;
    }

    // Obter informações do select original
    const selectId = selectElement.id;
    const selectName = selectElement.name;
    const isRequired = selectElement.hasAttribute('required');
    const isDisabled = selectElement.disabled;
    const currentValue = selectElement.value;

    // Obter as opções
    const options = Array.from(selectElement.options).map(option => ({
        value: option.value,
        text: option.text,
        icon: option.dataset.icon || null,
        selected: option.selected
    }));

    // Criar estrutura do custom select
    const wrapper = document.createElement('div');
    wrapper.className = 'custom-select-wrapper';
    wrapper.dataset.selectId = selectId;

    // Input hidden para manter compatibilidade com formulários
    const hiddenInput = document.createElement('input');
    hiddenInput.type = 'hidden';
    hiddenInput.id = selectId;
    hiddenInput.name = selectName;
    hiddenInput.value = currentValue;
    if (isRequired) {
        hiddenInput.required = true;
    }

    // Trigger (botão que abre o dropdown)
    const trigger = document.createElement('div');
    trigger.className = 'custom-select-trigger';
    if (isDisabled) {
        trigger.classList.add('disabled');
    }
    trigger.setAttribute('tabindex', isDisabled ? '-1' : '0');

    // Valor selecionado
    const valueContainer = document.createElement('div');
    valueContainer.className = 'custom-select-value';

    const selectedOption = options.find(opt => opt.value === currentValue);
    if (selectedOption && selectedOption.value !== '') {
        if (selectedOption.icon) {
            const icon = document.createElement('i');
            icon.className = selectedOption.icon;
            valueContainer.appendChild(icon);
        }
        const text = document.createElement('span');
        text.textContent = selectedOption.text;
        valueContainer.appendChild(text);
    } else {
        valueContainer.classList.add('placeholder');
        valueContainer.textContent = 'Selecione...';
    }

    // Seta
    const arrow = document.createElement('i');
    arrow.className = 'fas fa-chevron-down custom-select-arrow';

    trigger.appendChild(valueContainer);
    trigger.appendChild(arrow);

    // Dropdown
    const dropdown = document.createElement('div');
    dropdown.className = 'custom-select-dropdown';

    options.forEach(option => {
        if (option.value === '') return; // Pular opção placeholder

        const optionElement = document.createElement('div');
        optionElement.className = 'custom-select-option';
        optionElement.dataset.value = option.value;

        if (option.value === currentValue) {
            optionElement.classList.add('selected');
        }

        if (option.icon) {
            const icon = document.createElement('i');
            icon.className = option.icon;
            optionElement.appendChild(icon);
        }

        const text = document.createElement('span');
        text.textContent = option.text;
        optionElement.appendChild(text);

        // Event listener para selecionar opção
        optionElement.addEventListener('click', function() {
            selectOption(wrapper, option, optionElement);
        });

        dropdown.appendChild(optionElement);
    });

    // Montar estrutura
    wrapper.appendChild(hiddenInput);
    wrapper.appendChild(trigger);
    wrapper.appendChild(dropdown);

    // Substituir select original
    selectElement.style.display = 'none';
    selectElement.dataset.customized = 'true';
    selectElement.parentNode.insertBefore(wrapper, selectElement);

    // Event listeners
    if (!isDisabled) {
        trigger.addEventListener('click', function(e) {
            e.stopPropagation();
            toggleDropdown(wrapper);
        });

        // Navegação por teclado
        trigger.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleDropdown(wrapper);
            } else if (e.key === 'Escape') {
                closeDropdown(wrapper);
            }
        });
    }

    // Fechar dropdown ao clicar fora
    document.addEventListener('click', function(e) {
        if (!wrapper.contains(e.target)) {
            closeDropdown(wrapper);
        }
    });
}

function toggleDropdown(wrapper) {
    const trigger = wrapper.querySelector('.custom-select-trigger');
    const dropdown = wrapper.querySelector('.custom-select-dropdown');

    const isActive = trigger.classList.contains('active');

    // Fechar todos os outros dropdowns
    document.querySelectorAll('.custom-select-wrapper.active').forEach(w => {
        if (w !== wrapper) {
            closeDropdown(w);
        }
    });

    if (isActive) {
        closeDropdown(wrapper);
    } else {
        openDropdown(wrapper);
    }
}

function openDropdown(wrapper) {
    const trigger = wrapper.querySelector('.custom-select-trigger');
    const dropdown = wrapper.querySelector('.custom-select-dropdown');

    wrapper.classList.add('active'); // Adicionar active no wrapper para z-index alto
    trigger.classList.add('active');
    dropdown.classList.add('active');

    // Calcular posição do dropdown usando position: fixed
    positionDropdown(trigger, dropdown);

    // Reposicionar ao fazer scroll ou resize
    const repositionHandler = () => positionDropdown(trigger, dropdown);
    window.addEventListener('scroll', repositionHandler, true);
    window.addEventListener('resize', repositionHandler);

    // Remover listeners quando fechar
    dropdown.dataset.repositionHandler = 'attached';
}

function closeDropdown(wrapper) {
    const trigger = wrapper.querySelector('.custom-select-trigger');
    const dropdown = wrapper.querySelector('.custom-select-dropdown');

    wrapper.classList.remove('active'); // Remover active do wrapper
    trigger.classList.remove('active');
    dropdown.classList.remove('active');

    // Remover event listeners de scroll e resize
    if (dropdown.dataset.repositionHandler === 'attached') {
        window.removeEventListener('scroll', positionDropdown, true);
        window.removeEventListener('resize', positionDropdown);
        delete dropdown.dataset.repositionHandler;
    }
}

/**
 * Calcula e aplica a posição do dropdown usando position: fixed
 */
function positionDropdown(trigger, dropdown) {
    const rect = trigger.getBoundingClientRect();
    const viewportHeight = window.innerHeight;
    const dropdownHeight = dropdown.offsetHeight || 250; // altura max padrão

    // Calcular posição
    const spaceBelow = viewportHeight - rect.bottom;
    const spaceAbove = rect.top;

    // Decidir se abre para cima ou para baixo
    const openUpwards = spaceBelow < dropdownHeight && spaceAbove > spaceBelow;

    if (openUpwards) {
        // Abrir para cima
        dropdown.style.top = 'auto';
        dropdown.style.bottom = (viewportHeight - rect.top + 1) + 'px';
        dropdown.style.borderRadius = '6px 6px 0 0';
        dropdown.style.borderTop = '1px solid #ced4da';
        dropdown.style.borderBottom = 'none';
    } else {
        // Abrir para baixo (padrão)
        dropdown.style.top = (rect.bottom - 1) + 'px';
        dropdown.style.bottom = 'auto';
        dropdown.style.borderRadius = '0 0 6px 6px';
        dropdown.style.borderTop = 'none';
        dropdown.style.borderBottom = '1px solid #ced4da';
    }

    // Posição horizontal
    dropdown.style.left = rect.left + 'px';
    dropdown.style.width = rect.width + 'px';
    dropdown.style.minWidth = rect.width + 'px';
}

function selectOption(wrapper, option, optionElement) {
    const hiddenInput = wrapper.querySelector('input[type="hidden"]');
    const valueContainer = wrapper.querySelector('.custom-select-value');
    const dropdown = wrapper.querySelector('.custom-select-dropdown');

    // Atualizar valor
    hiddenInput.value = option.value;

    // Disparar evento change
    const changeEvent = new Event('change', { bubbles: true });
    hiddenInput.dispatchEvent(changeEvent);

    // Atualizar visual
    valueContainer.innerHTML = '';
    valueContainer.classList.remove('placeholder');

    if (option.icon) {
        const icon = document.createElement('i');
        icon.className = option.icon;
        valueContainer.appendChild(icon);
    }

    const text = document.createElement('span');
    text.textContent = option.text;
    valueContainer.appendChild(text);

    // Atualizar opção selecionada no dropdown
    dropdown.querySelectorAll('.custom-select-option').forEach(opt => {
        opt.classList.remove('selected');
    });
    optionElement.classList.add('selected');

    // Fechar dropdown
    closeDropdown(wrapper);
}

// Expor função globalmente para ser chamada após atualizações AJAX
window.initializeCustomSelects = initializeCustomSelects;
