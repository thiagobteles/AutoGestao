// ===================================================================
// PATCH PARA GRID-ENHANCED.JS
// Atualiza o método createDropdownElement para usar o novo sistema
// ===================================================================

/**
 * Este arquivo deve ser incluído APÓS o grid-enhanced.js
 * Ele sobrescreve o método createDropdownElement para suportar
 * diferentes tipos de requisição HTTP (GET, POST, PUT, DELETE)
 */

(function() {
    'use strict';

    console.log('🔧 Aplicando patch no GridActionsManager...');

    // Aguardar o GridActionsManager estar disponível
    function waitForGridActionsManager() {
        if (window.GridActionsManager && window.gridActionsManager) {
            applyPatch();
        } else {
            setTimeout(waitForGridActionsManager, 100);
        }
    }

    function applyPatch() {
        // Salvar método original como backup
        const originalCreateDropdownElement = window.GridActionsManager.prototype.createDropdownElement;

        // Sobrescrever o método
        window.GridActionsManager.prototype.createDropdownElement = function(actions, position) {
            // Remover dropdown anterior se existir
            if (this.dropdownElement) {
                this.dropdownElement.remove();
            }

            // Criar novo dropdown
            this.dropdownElement = document.createElement('div');
            this.dropdownElement.className = `dropdown-menu-portal direction-${position.direction}`;
            this.dropdownElement.style.setProperty('position', 'fixed', 'important');
            this.dropdownElement.style.setProperty('top', `${position.top - 50}px`, 'important');
            this.dropdownElement.style.setProperty('left', `${position.left}px`, 'important');

            // Adicionar itens
            actions.forEach((action, index) => {
                if (action.name === 'divider') {
                    const divider = document.createElement('hr');
                    divider.className = 'dropdown-divider-portal';
                    this.dropdownElement.appendChild(divider);
                } else {
                    const item = document.createElement('a');
                    item.href = action.url || '#';
                    item.className = `dropdown-item-portal ${action.cssClass || ''}`;
                    item.innerHTML = `<i class="${action.icon}"></i> ${action.displayName}`;

                    // Adicionar atributos de dados para o handler
                    // Aceita tanto número quanto string
                    item.setAttribute('data-action-name', action.name);
                    item.setAttribute('data-action-type', action.type !== undefined ? action.type : 0);

                    // Event listener para ações usando o novo sistema
                    item.addEventListener('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();

                        // Usar o novo sistema de execução de ações
                        if (window.executeGridAction) {
                            window.executeGridAction(action, e);
                        } else {
                            console.error('executeGridAction não está disponível');
                            // Fallback para método antigo
                            let actionType = 'get';

                            // Normalizar tipo (aceita número ou string)
                            if (typeof action.type === 'number') {
                                switch (action.type) {
                                    case 0: actionType = 'get'; break;
                                    case 1: actionType = 'post'; break;
                                    case 2: actionType = 'put'; break;
                                    case 3: actionType = 'delete'; break;
                                    default: actionType = 'get';
                                }
                            } else if (typeof action.type === 'string') {
                                actionType = action.type.toLowerCase();
                            }

                            if (actionType === 'get') {
                                window.location.href = action.url;
                            } else if (actionType === 'post') {
                                // Fallback básico para POST
                                if (action.name.toLowerCase().includes('delete') || action.name.toLowerCase().includes('excluir')) {
                                    if (confirm('Tem certeza que deseja excluir este registro?')) {
                                        this.executePostRequest(action.url);
                                    }
                                } else {
                                    this.executePostRequest(action.url);
                                }
                            }
                        }

                        // Fechar dropdown
                        this.closeDropdown();
                    });

                    this.dropdownElement.appendChild(item);
                }
            });

            // Adicionar ao portal
            const portal = document.getElementById('dropdownPortal') || document.body;
            portal.appendChild(this.dropdownElement);

            // Animar entrada
            setTimeout(() => {
                if (this.dropdownElement) {
                    this.dropdownElement.classList.add('show');
                }
            }, 10);

            console.log('✅ Dropdown criado com suporte a tipos de requisição HTTP');
        };

        // Adicionar método auxiliar para executar POST (fallback)
        window.GridActionsManager.prototype.executePostRequest = async function(url) {
            try {
                if (typeof showLoading === 'function') {
                    showLoading(true);
                }

                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest',
                        ...(token && { 'RequestVerificationToken': token })
                    },
                    body: JSON.stringify({})
                });

                if (!response.ok) {
                    throw new Error(`Erro: ${response.status}`);
                }

                const result = await response.json();

                if (result.success || result.sucesso) {
                    const message = result.message || result.mensagem || 'Operação realizada com sucesso!';
                    if (typeof showSuccess === 'function') {
                        await showSuccess(message);
                    }
                    window.location.reload();
                } else {
                    const errorMessage = result.message || result.mensagem || 'Erro ao executar operação';
                    if (typeof showError === 'function') {
                        showError(errorMessage);
                    }
                }

            } catch (error) {
                console.error('Erro na requisição POST:', error);
                if (typeof showError === 'function') {
                    showError(`Erro: ${error.message}`);
                }
            } finally {
                if (typeof showLoading === 'function') {
                    showLoading(false);
                }
            }
        };

        console.log('✅ Patch aplicado com sucesso no GridActionsManager');
    }

    // Iniciar processo de espera
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', waitForGridActionsManager);
    } else {
        waitForGridActionsManager();
    }

})();
