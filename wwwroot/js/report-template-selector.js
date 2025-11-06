/**
 * Gerenciador de Seleção de Templates de Relatório
 * Exibe modal para escolher template quando há múltiplas opções
 */

(function() {
    'use strict';

    console.log('📝 Carregando report-template-selector.js...');

    // Helper para mostrar erros (com fallback se showError não existir)
    const mostrarErro = (mensagem) => {
        console.error('❌ ' + mensagem);
        if (typeof window.showError === 'function') {
            window.showError(mensagem);
        } else {
            alert(mensagem); // Fallback
        }
    };

    try {
        // Garantir que está disponível globalmente
        window.ReportTemplateSelector = {
    /**
     * Gerar relatório - verifica templates disponíveis e abre modal se necessário
     */
    async gerarRelatorio(controller, entityId, entityType) {
        try {
            // Buscar templates disponíveis para esta entidade
            const response = await fetch(`/ReportBuilder/GetTemplatesByEntity?entityType=${entityType}`);
            const result = await response.json();

            if (!result.success) {
                mostrarErro('Erro ao buscar templates: ' + result.message);
                return;
            }

            const templates = result.data;

            // Log para debug
            console.log('📊 Templates recebidos:', templates);
            if (templates && templates.length > 0) {
                console.log('📋 Estrutura do primeiro template:', templates[0]);
                console.log('🔑 Propriedades:', Object.keys(templates[0]));
            }

            // Se não houver templates, abrir tela de configuração
            if (!templates || templates.length === 0) {
                window.open(`/${controller}/GerarRelatorio/${entityId}`, '_blank');
                return;
            }

            // Se houver apenas um template, gerar diretamente
            if (templates.length === 1) {
                const templateId = templates[0].id || templates[0].Id;
                console.log('📄 Gerando com template único. ID:', templateId);
                window.open(`/${controller}/GerarRelatorioComTemplate?id=${entityId}&templateId=${templateId}`, '_blank');
                return;
            }

            // Se houver múltiplos templates, mostrar modal de seleção
            console.log('📋 Mostrando modal com', templates.length, 'templates');
            this.showTemplateSelectionModal(controller, entityId, templates);
        } catch (error) {
            console.error('Erro ao gerar relatório:', error);
            mostrarErro('Erro ao processar solicitação de relatório');
        }
    },

    /**
     * Exibir modal de seleção de templates
     */
    showTemplateSelectionModal(controller, entityId, templates) {
        console.log('🎨 Criando modal de seleção com', templates.length, 'templates');

        // Normalizar propriedades (suportar camelCase e PascalCase)
        const normalizedTemplates = templates.map(t => ({
            id: t.id || t.Id,
            nome: t.nome || t.Nome,
            descricao: t.descricao || t.Descricao,
            Padrao: t.padrao || t.Padrao
        }));

        console.log('✅ Templates normalizados:', normalizedTemplates);

        // Criar HTML do modal
        const modalHtml = `
            <div class="modal fade" id="templateSelectorModal" tabindex="-1">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header bg-gradient-primary text-white">
                            <h5 class="modal-title">
                                <i class="fas fa-file-pdf me-2"></i>
                                Selecione o Relatório
                            </h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body p-4">
                            <p class="text-muted mb-3">
                                Foram encontrados ${normalizedTemplates.length} templates.<br>Selecione o relatório desejado:
                            </p>
                            <div class="template-list">
                                ${normalizedTemplates.map(t => `
                                    <div class="template-item" onclick="ReportTemplateSelector.selectTemplate('${controller}', ${entityId}, ${t.id})" data-template-id="${t.id}">
                                        <div class="template-icon">
                                            <i class="fas fa-file-alt"></i>
                                        </div>
                                        <div class="template-info">
                                            <div class="template-name">
                                                ${t.nome || 'Template sem nome'}
                                                ${t.Padrao ? '<span class="badge bg-primary ms-2">Padrão</span>' : ''}
                                            </div>
                                            ${t.descricao ? `<div class="template-description">${t.descricao}</div>` : '<div class="template-description text-muted">Sem descrição</div>'}
                                        </div>
                                        <div class="template-arrow">
                                            <i class="fas fa-chevron-right"></i>
                                        </div>
                                    </div>
                                `).join('')}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Remover modal anterior se existir
        const existingModal = document.getElementById('templateSelectorModal');
        if (existingModal) {
            existingModal.remove();
        }

        // Adicionar modal ao body
        document.body.insertAdjacentHTML('beforeend', modalHtml);

        // Abrir modal
        const modal = new bootstrap.Modal(document.getElementById('templateSelectorModal'));
        modal.show();

        // Remover modal do DOM quando fechar
        document.getElementById('templateSelectorModal').addEventListener('hidden.bs.modal', function() {
            this.remove();
        });
    },

    /**
     * Selecionar template e gerar relatório
     */
    selectTemplate(controller, entityId, templateId) {
        console.log('🎯 Template selecionado:', {
            controller,
            entityId,
            templateId
        });

        // Fechar modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('templateSelectorModal'));
        if (modal) {
            modal.hide();
        }

        // Validar parâmetros
        if (!templateId) {
            console.error('❌ TemplateId está undefined!');
            mostrarErro('Erro: ID do template não foi identificado.');
            return;
        }

        // Abrir relatório em nova aba
        const url = `/${controller}/GerarRelatorioComTemplate?id=${entityId}&templateId=${templateId}`;
        console.log('🌐 Abrindo URL:', url);
        window.open(url, '_blank');
    }
};

// Adicionar estilos CSS para o modal
const style = document.createElement('style');
style.textContent = `
    .bg-gradient-primary {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .template-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
    }

    .template-item {
        display: flex;
        align-items: center;
        gap: 15px;
        padding: 15px;
        background: #f8f9fa;
        border: 2px solid #e9ecef;
        border-radius: 10px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .template-item:hover {
        background: #e7f3ff;
        border-color: #667eea;
        transform: translateX(5px);
        box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
    }

    .template-icon {
        width: 50px;
        height: 50px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-radius: 10px;
        font-size: 24px;
    }

    .template-info {
        flex: 1;
    }

    .template-name {
        font-weight: 700;
        font-size: 16px;
        color: #2c3e50;
        margin-bottom: 4px;
    }

    .template-description {
        font-size: 13px;
        color: #6c757d;
        line-height: 1.4;
    }

    .template-arrow {
        color: #667eea;
        font-size: 18px;
        transition: transform 0.3s ease;
    }

    .template-item:hover .template-arrow {
        transform: translateX(5px);
    }

    .badge {
        font-size: 11px;
        padding: 4px 8px;
        font-weight: 600;
    }
`;
        document.head.appendChild(style);

        // Log de sucesso
        console.log('✅ ReportTemplateSelector carregado com sucesso!');
        console.log('✅ Disponível globalmente:', typeof window.ReportTemplateSelector !== 'undefined');

    } catch (error) {
        console.error('❌ ERRO CRÍTICO ao carregar report-template-selector.js:', error);
        console.error('Stack trace:', error.stack);

        // Garantir que o objeto existe mesmo com erro
        if (!window.ReportTemplateSelector) {
            window.ReportTemplateSelector = {
                gerarRelatorio: function() {
                    alert('Erro: Módulo de relatórios com falha. Detalhes no console.');
                    console.error('ReportTemplateSelector não pôde ser inicializado corretamente.');
                }
            };
        }
    }
})();
