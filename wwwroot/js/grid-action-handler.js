// ===================================================================
// GRID ACTION HANDLER - Processa tipo de requisição HTTP (GET/POST/PUT/DELETE)
// ===================================================================

/**
 * Executa uma ação baseada no tipo de requisição HTTP configurado
 * @param {Object} action - Objeto da ação contendo url, type, name, displayName
 * @param {Event} event - Evento do clique
 */
async function executeGridAction(action, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    try {
        const url = action.url;

        if (!url) {
            console.error('URL não definida para a ação:', action);
            showError('Erro: URL da ação não configurada');
            return;
        }

        // Normalizar tipo de requisição (aceita número ou string)
        // 0 = GET, 1 = POST, 2 = PUT, 3 = DELETE
        let requestType = 'get';
        if (typeof action.type === 'number') {
            switch (action.type) {
                case 0: requestType = 'get'; break;
                case 1: requestType = 'post'; break;
                case 2: requestType = 'put'; break;
                case 3: requestType = 'delete'; break;
                default: requestType = 'get';
            }
        } else if (typeof action.type === 'string') {
            requestType = action.type.toLowerCase();
        }

        console.log(`🚀 Executando ação: ${action.name} | Tipo: ${requestType} (original: ${action.type}) | URL: ${url}`);

        // Processar baseado no tipo de requisição
        switch (requestType) {
            case 'get':
                await executeGetAction(url, action);
                break;

            case 'post':
                await executePostAction(url, action);
                break;

            case 'put':
                await executePutAction(url, action);
                break;

            case 'delete':
                await executeDeleteAction(url, action);
                break;

            default:
                console.warn(`Tipo de requisição desconhecido: ${requestType}. Usando GET como padrão.`);
                await executeGetAction(url, action);
                break;
        }

    } catch (error) {
        console.error('Erro ao executar ação:', error);
        showError(`Erro ao executar ação: ${error.message}`);
    }
}

/**
 * Executa uma ação GET (navegação normal)
 */
async function executeGetAction(url, action) {
    console.log(`📄 Executando GET para: ${url}`);
    window.location.href = url;
}

/**
 * Executa uma ação POST (com confirmação para Delete)
 */
async function executePostAction(url, action) {
    const actionName = action.name.toLowerCase();
    const isDeleteAction = actionName.includes('delete') || actionName.includes('excluir');

    // Se for ação de exclusão, pedir confirmação
    if (isDeleteAction) {
        const confirmed = await showConfirm(
            'Tem certeza que deseja excluir este registro?',
            {
                title: 'Confirmar Exclusão',
                okText: 'Sim, Excluir',
                cancelText: 'Cancelar',
                type: 'danger'
            }
        );

        if (!confirmed) {
            console.log('❌ Exclusão cancelada pelo usuário');
            return;
        }
    }

    console.log(`📮 Executando POST para: ${url}`);

    // Mostrar loading
    if (typeof showLoading === 'function') {
        showLoading(true);
    }

    try {
        // Obter o token antiforgery
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
            throw new Error(`Erro na requisição: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();

        // Processar resposta
        if (result.success || result.sucesso) {
            const message = result.message || result.mensagem || 'Operação realizada com sucesso!';
            
            if (typeof showSuccess === 'function') {
                await showSuccess(message);
            } else {
                alert(message);
            }

            // Executar script se houver
            if (result.script) {
                eval(result.script);
            } else {
                // Recarregar a página
                window.location.reload();
            }
        } else {
            const errorMessage = result.message || result.mensagem || 'Erro ao executar a operação';
            
            if (typeof showError === 'function') {
                showError(errorMessage);
            } else {
                alert(errorMessage);
            }
        }

    } catch (error) {
        console.error('Erro na requisição POST:', error);
        
        if (typeof showError === 'function') {
            showError(`Erro ao executar operação: ${error.message}`);
        } else {
            alert(`Erro ao executar operação: ${error.message}`);
        }
    } finally {
        if (typeof showLoading === 'function') {
            showLoading(false);
        }
    }
}

/**
 * Executa uma ação PUT
 */
async function executePutAction(url, action) {
    console.log(`🔄 Executando PUT para: ${url}`);

    if (typeof showLoading === 'function') {
        showLoading(true);
    }

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch(url, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                ...(token && { 'RequestVerificationToken': token })
            },
            body: JSON.stringify({})
        });

        if (!response.ok) {
            throw new Error(`Erro na requisição: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();

        if (result.success || result.sucesso) {
            const message = result.message || result.mensagem || 'Operação realizada com sucesso!';
            
            if (typeof showSuccess === 'function') {
                await showSuccess(message);
            } else {
                alert(message);
            }

            window.location.reload();
        } else {
            const errorMessage = result.message || result.mensagem || 'Erro ao executar a operação';
            
            if (typeof showError === 'function') {
                showError(errorMessage);
            } else {
                alert(errorMessage);
            }
        }

    } catch (error) {
        console.error('Erro na requisição PUT:', error);
        
        if (typeof showError === 'function') {
            showError(`Erro ao executar operação: ${error.message}`);
        } else {
            alert(`Erro ao executar operação: ${error.message}`);
        }
    } finally {
        if (typeof showLoading === 'function') {
            showLoading(false);
        }
    }
}

/**
 * Executa uma ação DELETE (método HTTP DELETE)
 */
async function executeDeleteAction(url, action) {
    const confirmed = await showConfirm(
        'Tem certeza que deseja excluir este registro?',
        {
            title: 'Confirmar Exclusão',
            okText: 'Sim, Excluir',
            cancelText: 'Cancelar',
            type: 'danger'
        }
    );

    if (!confirmed) {
        console.log('❌ Exclusão cancelada pelo usuário');
        return;
    }

    console.log(`🗑️ Executando DELETE para: ${url}`);

    if (typeof showLoading === 'function') {
        showLoading(true);
    }

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const response = await fetch(url, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                ...(token && { 'RequestVerificationToken': token })
            }
        });

        if (!response.ok) {
            throw new Error(`Erro na requisição: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();

        if (result.success || result.sucesso) {
            const message = result.message || result.mensagem || 'Registro excluído com sucesso!';
            
            if (typeof showSuccess === 'function') {
                await showSuccess(message);
            } else {
                alert(message);
            }

            window.location.reload();
        } else {
            const errorMessage = result.message || result.mensagem || 'Erro ao excluir registro';
            
            if (typeof showError === 'function') {
                showError(errorMessage);
            } else {
                alert(errorMessage);
            }
        }

    } catch (error) {
        console.error('Erro na requisição DELETE:', error);
        
        if (typeof showError === 'function') {
            showError(`Erro ao executar operação: ${error.message}`);
        } else {
            alert(`Erro ao executar operação: ${error.message}`);
        }
    } finally {
        if (typeof showLoading === 'function') {
            showLoading(false);
        }
    }
}

// ===================================================================
// INTEGRAÇÃO COM GRID-ENHANCED.JS
// ===================================================================

/**
 * Atualiza o método createDropdownElement do GridActionsManager
 * para usar o novo sistema de execução de ações
 */
document.addEventListener('DOMContentLoaded', function() {
    console.log('🔧 Inicializando Grid Action Handler...');

    // Adicionar listener global para ações do dropdown
    document.addEventListener('click', function(e) {
        const dropdownItem = e.target.closest('.dropdown-item-portal');

        if (dropdownItem) {
            e.preventDefault();
            e.stopPropagation();

            // Obter dados da ação do elemento
            let actionType = dropdownItem.getAttribute('data-action-type') || '0';

            // Converter para número se for string numérica, senão mantém a string
            if (!isNaN(actionType)) {
                actionType = parseInt(actionType, 10);
            }

            const actionData = {
                name: dropdownItem.getAttribute('data-action-name') || '',
                displayName: dropdownItem.textContent.trim(),
                url: dropdownItem.getAttribute('href'),
                type: actionType
            };

            // Executar a ação
            executeGridAction(actionData, e);

            // Fechar dropdown
            if (window.gridActionsManager) {
                window.gridActionsManager.closeDropdown();
            }
        }
    });

    console.log('✅ Grid Action Handler inicializado');
});

// Exportar para uso global
window.executeGridAction = executeGridAction;
