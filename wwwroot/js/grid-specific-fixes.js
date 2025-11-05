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
function restoreGridDoubleClick() {
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
                let controller = window.gridControllerResolver.getCurrentController();

                // Abrir edição
                if (controller && entityId) {
                    if (controller === 'ReportTemplate') {
                        window.location.href = `/ReportBuilder/Edit/${entityId}`;
                    } else {
                        window.location.href = `/${controller}/Details/${entityId}`;
                    }
                }
            });

            // Adicionar cursor pointer para indicar clicável
            row.style.cursor = 'pointer';
            row.title = 'Clique duplo para visualizar';
        });
    });
}

// ===================================================================
// 3. CORREÇÃO PARA BOTÕES DE AÇÃO (DROPDOWN)
// ===================================================================
function restoreGridActions() {

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
function fixOnclickConfirms() {
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
    // Aguardar um pouco para garantir que outros scripts carregaram
    setTimeout(function () {
        restoreGridDoubleClick();
        restoreGridActions();
        fixOnclickConfirms();
    }, 1000);
});

// Executar também após AJAX/atualizações da grid
document.addEventListener('gridUpdated', function () {
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
};