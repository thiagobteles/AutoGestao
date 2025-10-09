/**
 * Helper JavaScript para Sistema de Relatórios
 * Facilita a geração de relatórios em qualquer tela
 */

const ReportHelper = {
    /**
     * Gera relatório rápido
     * @param {string} entityType - Tipo da entidade (Cliente, Veiculo, etc)
     * @param {number} entityId - ID do registro
     */
    quickReport(entityType, entityId) {
        const url = `/Report/Quick?entityType=${entityType}&entityId=${entityId}`;
        window.open(url, '_blank');
    },

    /**
     * Abre builder de relatório
     * @param {string} entityType - Tipo da entidade
     * @param {number} entityId - ID do registro
     */
    customReport(entityType, entityId) {
        const url = `/Report/Builder?entityType=${entityType}&entityId=${entityId}`;
        window.open(url, '_blank');
    },

    /**
     * Gera relatório usando template salvo
     * @param {number} templateId - ID do template
     * @param {number} entityId - ID do registro
     */
    fromTemplate(templateId, entityId) {
        const url = `/Report/GenerateFromSaved?templateId=${templateId}&entityId=${entityId}`;
        window.open(url, '_blank');
    },

    /**
     * Busca templates disponíveis para uma entidade
     * @param {string} entityType - Tipo da entidade
     * @returns {Promise<Array>} Lista de templates
     */
    async getTemplates(entityType) {
        try {
            const response = await fetch(`/Report/GetSavedTemplates?entityType=${entityType}`);
            if (!response.ok) throw new Error('Erro ao buscar templates');
            return await response.json();
        } catch (error) {
            console.error('Erro ao buscar templates:', error);
            return [];
        }
    },

    /**
     * Mostra modal com opções de relatório
     * @param {string} entityType - Tipo da entidade
     * @param {number} entityId - ID do registro
     */
    async showReportModal(entityType, entityId) {
        const templates = await this.getTemplates(entityType);

        const modalHtml = `
            <div class="modal fade" id="reportModal" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <i class="fas fa-file-pdf me-2"></i>
                                Gerar Relatório
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <h6 class="mb-3">Escolha uma opção:</h6>
                            
                            <div class="list-group">
                                <button type="button" class="list-group-item list-group-item-action" 
                                        onclick="ReportHelper.quickReport('${entityType}', ${entityId}); bootstrap.Modal.getInstance(document.getElementById('reportModal')).hide();">
                                    <i class="fas fa-bolt me-2 text-primary"></i>
                                    <strong>Relatório Rápido</strong>
                                    <small class="d-block text-muted">Gerar relatório padrão imediatamente</small>
                                </button>

                                <button type="button" class="list-group-item list-group-item-action" 
                                        onclick="ReportHelper.customReport('${entityType}', ${entityId}); bootstrap.Modal.getInstance(document.getElementById('reportModal')).hide();">
                                    <i class="fas fa-cog me-2 text-info"></i>
                                    <strong>Personalizar Relatório</strong>
                                    <small class="d-block text-muted">Configurar campos e seções manualmente</small>
                                </button>
                            </div>

                            ${templates.length > 0 ? `
                                <hr>
                                <h6 class="mb-3">Templates Salvos:</h6>
                                <div class="list-group">
                                    ${templates.map(t => `
                                        <button type="button" class="list-group-item list-group-item-action" 
                                                onclick="ReportHelper.fromTemplate(${t.Id}, ${entityId}); bootstrap.Modal.getInstance(document.getElementById('reportModal')).hide();">
                                            <i class="fas fa-file-alt me-2 text-success"></i>
                                            <strong>${t.Nome}</strong>
                                            ${t.IsPadrao ? '<span class="badge bg-primary ms-2">Padrão</span>' : ''}
                                            ${t.Descricao ? `<small class="d-block text-muted">${t.Descricao}</small>` : ''}
                                        </button>
                                    `).join('')}
                                </div>
                            ` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Remover modal antigo se existir
        const oldModal = document.getElementById('reportModal');
        if (oldModal) oldModal.remove();

        // Adicionar novo modal
        document.body.insertAdjacentHTML('beforeend', modalHtml);

        // Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('reportModal'));
        modal.show();
    },

    /**
     * Adiciona botão de relatório em um elemento
     * @param {HTMLElement} container - Elemento onde adicionar o botão
     * @param {string} entityType - Tipo da entidade
     * @param {number} entityId - ID do registro
     */
    addReportButton(container, entityType, entityId) {
        const button = document.createElement('button');
        button.className = 'btn btn-outline-success';
        button.innerHTML = '<i class="fas fa-file-pdf me-2"></i>Gerar Relatório';
        button.onclick = () => this.showReportModal(entityType, entityId);
        container.appendChild(button);
    },

    /**
     * Exportar dados para CSV (alternativa ao PDF)
     * @param {Array} data - Dados para exportar
     * @param {string} filename - Nome do arquivo
     */
    exportToCSV(data, filename = 'export.csv') {
        if (!data || data.length === 0) {
            alert('Nenhum dado para exportar');
            return;
        }

        const headers = Object.keys(data[0]);
        const csv = [
            headers.join(','),
            ...data.map(row =>
                headers.map(header => {
                    const value = row[header];
                    return typeof value === 'string' && value.includes(',')
                        ? `"${value}"`
                        : value;
                }).join(',')
            )
        ].join('\n');

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        link.click();
    },

    /**
     * Gerar relatório em lote (múltiplos registros)
     * @param {string} entityType - Tipo da entidade
     * @param {Array<number>} entityIds - IDs dos registros
     */
    batchReport(entityType, entityIds) {
        if (!entityIds || entityIds.length === 0) {
            alert('Selecione ao menos um registro');
            return;
        }

        if (entityIds.length > 50) {
            if (!confirm('Gerar mais de 50 relatórios pode demorar. Deseja continuar?')) {
                return;
            }
        }

        // Abrir cada relatório em nova aba (com delay para não sobrecarregar)
        entityIds.forEach((id, index) => {
            setTimeout(() => {
                this.quickReport(entityType, id);
            }, index * 500); // 500ms de intervalo entre cada
        });
    }
};

// Disponibilizar globalmente
window.ReportHelper = ReportHelper;

// ==================================================
// EXEMPLOS DE USO
// ==================================================

/*
// 1. Gerar relatório rápido
ReportHelper.quickReport('Cliente', 123);

// 2. Abrir builder personalizado
ReportHelper.customReport('Veiculo', 456);

// 3. Usar template salvo
ReportHelper.fromTemplate(1, 789);

// 4. Mostrar modal com opções
ReportHelper.showReportModal('Cliente', 123);

// 5. Adicionar botão em uma página
const container = document.getElementById('actions');
ReportHelper.addReportButton(container, 'Cliente', 123);

// 6. Buscar templates disponíveis
const templates = await ReportHelper.getTemplates('Cliente');
console.log(templates);

// 7. Exportar para CSV
const dados = [
    { nome: 'João', email: 'joao@email.com' },
    { nome: 'Maria', email: 'maria@email.com' }
];
ReportHelper.exportToCSV(dados, 'clientes.csv');

// 8. Gerar múltiplos relatórios
const ids = [1, 2, 3, 4, 5];
ReportHelper.batchReport('Cliente', ids);
*/

// ==================================================
// INTEGRAÇÃO COM GRID
// ==================================================

/**
 * Adicionar ação em massa para relatórios na grid
 */
function addBulkReportAction() {
    // Buscar IDs selecionados (assumindo que há checkboxes)
    const selectedIds = Array.from(document.querySelectorAll('.row-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    if (selectedIds.length === 0) {
        alert('Selecione ao menos um registro');
        return;
    }

    // Obter tipo de entidade da página
    const entityType = document.querySelector('[data-entity-type]')?.dataset.entityType || 'Cliente';

    // Gerar relatórios
    ReportHelper.batchReport(entityType, selectedIds);
}

// ==================================================
// ATALHOS DE TECLADO
// ==================================================

document.addEventListener('keydown', function (e) {
    // Ctrl + P = Gerar relatório do registro atual
    if (e.ctrlKey && e.key === 'p') {
        e.preventDefault();

        // Tentar encontrar ID na URL
        const match = window.location.pathname.match(/\/(\d+)$/);
        if (match) {
            const id = parseInt(match[1]);
            const entityType = window.location.pathname.split('/')[1];
            ReportHelper.quickReport(entityType, id);
        }
    }
});

// ==================================================
// NOTIFICAÇÕES
// ==================================================

const ReportNotifications = {
    success(message) {
        this.show(message, 'success');
    },

    error(message) {
        this.show(message, 'danger');
    },

    info(message) {
        this.show(message, 'info');
    },

    show(message, type = 'info') {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3" 
                 style="z-index: 9999; min-width: 300px;" role="alert">
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'danger' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', alertHtml);

        // Auto-remover após 5 segundos
        setTimeout(() => {
            const alert = document.querySelector('.alert');
            if (alert) {
                alert.classList.remove('show');
                setTimeout(() => alert.remove(), 150);
            }
        }, 5000);
    }
};

window.ReportNotifications = ReportNotifications;

// ==================================================
// PREVIEW DE RELATÓRIO (sem abrir nova aba)
// ==================================================

const ReportPreview = {
    async show(entityType, entityId) {
        try {
            const response = await fetch(`/Report/Quick?entityType=${entityType}&entityId=${entityId}`);
            const html = await response.text();

            // Criar modal com preview
            const modalHtml = `
                <div class="modal fade" id="reportPreviewModal" tabindex="-1">
                    <div class="modal-dialog modal-xl">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">
                                    <i class="fas fa-eye me-2"></i>
                                    Preview do Relatório
                                </h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body p-0">
                                <iframe srcdoc="${html.replace(/"/g, '&quot;')}" 
                                        style="width: 100%; height: 600px; border: none;"></iframe>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
                                <button type="button" class="btn btn-primary" onclick="ReportHelper.quickReport('${entityType}', ${entityId})">
                                    <i class="fas fa-print me-2"></i>
                                    Abrir para Imprimir
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;

            // Remover modal antigo
            const oldModal = document.getElementById('reportPreviewModal');
            if (oldModal) oldModal.remove();

            // Adicionar e mostrar
            document.body.insertAdjacentHTML('beforeend', modalHtml);
            const modal = new bootstrap.Modal(document.getElementById('reportPreviewModal'));
            modal.show();

        } catch (error) {
            ReportNotifications.error('Erro ao carregar preview: ' + error.message);
        }
    }
};

window.ReportPreview = ReportPreview;

// ==================================================
// LOADING STATE
// ==================================================

const ReportLoading = {
    show(message = 'Gerando relatório...') {
        const loadingHtml = `
            <div id="reportLoading" class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" 
                 style="background: rgba(0,0,0,0.7); z-index: 9999;">
                <div class="text-center text-white">
                    <div class="spinner-border mb-3" role="status">
                        <span class="visually-hidden">Carregando...</span>
                    </div>
                    <div>${message}</div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', loadingHtml);
    },

    hide() {
        const loading = document.getElementById('reportLoading');
        if (loading) loading.remove();
    }
};

window.ReportLoading = ReportLoading;

console.log('Report Helper carregado com sucesso!');