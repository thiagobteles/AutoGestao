/**
 * CORREÇÕES ESPECÍFICAS PARA PROBLEMAS IDENTIFICADOS
 * - Botão Limpar usando confirm() antigo
 * - Grid perdeu duplo clique
 * - Botão de ações parou de funcionar
 */

// ===================================================================
// 1. CORREÇÃO PARA BOTÕES LIMPAR - PADRÃO EM FORMULÁRIOS
// ===================================================================

// PROBLEMA: Botões "Limpar" ainda usam confirm() nativo
// SOLUÇÃO: Substituir por modal de confirmação

// JavaScript para substituir confirmações de limpeza
document.addEventListener('DOMContentLoaded', function () {
    // Interceptar cliques em botões de limpar
    document.addEventListener('click', async function (e) {
        // Verificar se é botão de limpar/cancelar
        if (e.target.matches('.btn-secondary, .btn-outline-secondary, [onclick*="confirm"], [onclick*="limpar"]') ||
            e.target.textContent.toLowerCase().includes('limpar') ||
            e.target.textContent.toLowerCase().includes('cancelar') ||
            e.target.closest('button')?.textContent.toLowerCase().includes('limpar')) {

            // Se tem onclick com confirm, interceptar
            const onclick = e.target.getAttribute('onclick') || e.target.closest('button')?.getAttribute('onclick');
            if (onclick && onclick.includes('confirm')) {
                e.preventDefault();
                e.stopPropagation();

                const confirmed = await showConfirm('Tem certeza que deseja limpar todos os campos?', {
                    title: 'Limpar Formulário',
                    okText: 'Sim, Limpar',
                    cancelText: 'Cancelar',
                    type: 'warning'
                });

                if (confirmed) {
                    // Executar ação original sem confirm()
                    const cleanOnclick = onclick.replace(/confirm\([^)]+\)\s*&&?\s*/, '');
                    eval(cleanOnclick);
                }
            }
        }
    });
});

// ===================================================================
// 2. CORREÇÃO PARA DUPLO CLIQUE NA GRID
// ===================================================================

// PROBLEMA: Grid perdeu funcionalidade de duplo clique
// SOLUÇÃO: Reativar eventos de duplo clique

function restoreGridDoubleClick() {
    console.log('🔄 Restaurando duplo clique da grid...');

    // Encontrar tabelas da grid
    const gridTables = document.querySelectorAll('.base-grid-table, .grid-table, table[data-grid="true"]');

    gridTables.forEach(table => {
        // Remover listeners antigos
        const rows = table.querySelectorAll('tbody tr');

        rows.forEach(row => {
            // Adicionar evento de duplo clique
            row.addEventListener('dblclick', function (e) {
                // Evitar conflito com botões de ação
                if (e.target.closest('.dropdown, .btn, button, a')) {
                    return;
                }

                // Procurar ID da linha
                const idCell = row.querySelector('[data-id], td:first-child');
                let entityId = null;

                if (idCell) {
                    entityId = idCell.getAttribute('data-id') || idCell.textContent.trim();
                }

                // Determinar controller baseado na URL
                const currentPath = window.location.pathname;
                let controller = '';

                if (currentPath.includes('Clientes')) controller = 'Clientes';
                else if (currentPath.includes('Veiculos')) controller = 'Veiculos';
                else if (currentPath.includes('Vendedores')) controller = 'Vendedores';
                else if (currentPath.includes('Fornecedores')) controller = 'Fornecedores';
                else if (currentPath.includes('Usuarios')) controller = 'Usuarios';

                // Abrir edição
                if (controller && entityId) {
                    window.location.href = `/${controller}/Edit/${entityId}`;
                }

                console.log(`📝 Duplo clique: ${controller}/${entityId}`);
            });

            // Adicionar cursor pointer para indicar clicável
            row.style.cursor = 'pointer';
            row.title = 'Clique duplo para editar';
        });
    });
}

// ===================================================================
// 3. CORREÇÃO PARA BOTÕES DE AÇÃO (DROPDOWN)
// ===================================================================

// PROBLEMA: Botões de ação pararam de funcionar
// SOLUÇÃO: Reativar dropdowns e eventos

function restoreGridActions() {
    console.log('🔄 Restaurando ações da grid...');

    // Reativar dropdowns Bootstrap
    const dropdownButtons = document.querySelectorAll('[data-bs-toggle="dropdown"], .dropdown-toggle');
    dropdownButtons.forEach(button => {
        // Garantir que Bootstrap dropdown funcione
        if (!button.getAttribute('data-bs-toggle')) {
            button.setAttribute('data-bs-toggle', 'dropdown');
        }

        // Recriar instância do dropdown se necessário
        try {
            new bootstrap.Dropdown(button);
        } catch (e) {
            console.log('Dropdown já inicializado ou Bootstrap não disponível');
        }
    });

    // Reativar cliques em ações
    document.addEventListener('click', function (e) {
        const target = e.target;

        // Ação de Visualizar
        if (target.matches('.action-view, [data-action="view"]') ||
            target.textContent.includes('Visualizar') ||
            target.querySelector('i.fa-eye')) {

            const id = getEntityIdFromRow(target);
            const controller = getCurrentController();
            if (id && controller) {
                window.location.href = `/${controller}/Details/${id}`;
            }
        }

        // Ação de Editar
        if (target.matches('.action-edit, [data-action="edit"]') ||
            target.textContent.includes('Editar') ||
            target.querySelector('i.fa-edit, i.fa-pencil')) {

            const id = getEntityIdFromRow(target);
            const controller = getCurrentController();
            if (id && controller) {
                window.location.href = `/${controller}/Edit/${id}`;
            }
        }

        // Ação de Excluir
        if (target.matches('.action-delete, [data-action="delete"]') ||
            target.textContent.includes('Excluir') ||
            target.querySelector('i.fa-trash, i.fa-times')) {

            e.preventDefault();
            const id = getEntityIdFromRow(target);
            if (id) {
                confirmarExclusao(id);
            }
        }
    });
}

// Função auxiliar para obter ID da entidade
function getEntityIdFromRow(element) {
    const row = element.closest('tr');
    if (!row) return null;

    // Procurar em data attributes
    let id = row.getAttribute('data-id');
    if (id) return id;

    // Procurar na primeira célula
    const firstCell = row.querySelector('td:first-child');
    if (firstCell) {
        id = firstCell.getAttribute('data-id') || firstCell.textContent.trim();
        if (id && !isNaN(id)) return id;
    }

    // Procurar em qualquer célula com data-id
    const cellWithId = row.querySelector('[data-id]');
    if (cellWithId) {
        return cellWithId.getAttribute('data-id');
    }

    return null;
}

// Função auxiliar para obter controller atual
function getCurrentController() {
    const path = window.location.pathname;
    const segments = path.split('/').filter(s => s);
    return segments[0] || null;
}

// ===================================================================
// 4. CORREÇÃO ESPECÍFICA PARA CONFIRMS EM ATRIBUTOS ONCLICK
// ===================================================================

// PROBLEMA: Elementos com onclick="confirm(...)" ainda usam confirm nativo
// SOLUÇÃO: Interceptar e substituir

function fixOnclickConfirms() {
    console.log('🔄 Corrigindo confirms em onclick...');

    // Buscar todos elementos com onclick contendo confirm
    const elementsWithConfirm = document.querySelectorAll('*[onclick*="confirm"]');

    elementsWithConfirm.forEach(element => {
        const originalOnclick = element.getAttribute('onclick');

        if (originalOnclick && originalOnclick.includes('confirm')) {
            // Remover onclick original
            element.removeAttribute('onclick');

            // Adicionar novo event listener
            element.addEventListener('click', async function (e) {
                e.preventDefault();
                e.stopPropagation();

                // Extrair mensagem do confirm
                const confirmMatch = originalOnclick.match(/confirm\(['"`]([^'"`]+)['"`]\)/);
                const message = confirmMatch ? confirmMatch[1] : 'Tem certeza que deseja continuar?';

                const confirmed = await showConfirm(message, {
                    title: 'Confirmação',
                    okText: 'Confirmar',
                    cancelText: 'Cancelar',
                    type: 'warning'
                });

                if (confirmed) {
                    // Executar código sem o confirm
                    const cleanCode = originalOnclick.replace(/confirm\([^)]+\)\s*&&?\s*/, '');
                    try {
                        eval(cleanCode);
                    } catch (error) {
                        console.error('Erro ao executar código:', error);
                    }
                }
            });
        }
    });
}

// ===================================================================
// 5. INICIALIZAÇÃO AUTOMÁTICA
// ===================================================================

// Executar correções quando DOM estiver pronto
document.addEventListener('DOMContentLoaded', function () {
    console.log('🔧 Iniciando correções específicas...');

    // Aguardar um pouco para garantir que outros scripts carregaram
    setTimeout(function () {
        restoreGridDoubleClick();
        restoreGridActions();
        fixOnclickConfirms();

        console.log('✅ Correções específicas aplicadas');
    }, 1000);
});

// Executar também após AJAX/atualizações da grid
document.addEventListener('gridUpdated', function () {
    console.log('🔄 Grid atualizada, reaplicando correções...');
    restoreGridDoubleClick();
    restoreGridActions();
    fixOnclickConfirms();
});

// ===================================================================
// 6. FUNÇÃO GLOBAL PARA REAPLICAR CORREÇÕES
// ===================================================================

window.applyGridFixes = function () {
    restoreGridDoubleClick();
    restoreGridActions();
    fixOnclickConfirms();
    console.log('✅ Correções da grid reaplicadas manualmente');
};

console.log('🛠️ Sistema de correções específicas carregado');