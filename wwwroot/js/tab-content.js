// wwwroot/js/tab-content.js - ARQUIVO COMPLETO CORRIGIDO
class TabContentManager {
    constructor() {
        this.activeTab = null;
        this.parentId = null;
        this.parentController = null;
    }

    init() {
        console.log('TabContentManager inicializado');
        this.setupEventListeners();
    }

    setupEventListeners() {
        document.addEventListener('click', (e) => {
            // Botão adicionar
            if (e.target.closest('.btn-add-tab-item')) {
                e.preventDefault();
                const btn = e.target.closest('.btn-add-tab-item');
                const tabId = btn.dataset.tab;
                const controller = btn.dataset.controller;
                const parentId = btn.dataset.parentId;
                this.openCreateModal(controller, parentId, tabId);
            }

            // Botão editar
            if (e.target.closest('.btn-edit-tab-item')) {
                e.preventDefault();
                const btn = e.target.closest('.btn-edit-tab-item');
                const id = btn.dataset.id;
                const controller = btn.dataset.controller;
                const parentId = btn.dataset.parentId;
                this.openEditModal(controller, id, parentId);
            }

            // Botão deletar
            if (e.target.closest('.btn-delete-tab-item')) {
                e.preventDefault();
                const btn = e.target.closest('.btn-delete-tab-item');
                const id = btn.dataset.id;
                const controller = btn.dataset.controller;
                this.deleteItem(controller, id);
            }
        });
    }

    async openCreateModal(controller, parentId, tabId) {
        try {
            console.log('Abrindo modal de criação:', { controller, parentId, tabId });

            const modal = this.createModalElement(controller, 'create');
            document.body.appendChild(modal);

            // Determinar o nome do campo pai baseado no controller
            const parentField = this.getParentFieldName(controller);
            let url = `/${controller}/Create?modal=true`;

            if (parentId && parentField) {
                url += `&${parentField}=${parentId}`;
            }

            console.log('URL de criação:', url);

            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao carregar formulário: ${response.status}`);
            }

            const html = await response.text();
            console.log('HTML recebido, tamanho:', html.length);

            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const saveBtn = modal.querySelector('#modalSaveBtn');
            if (saveBtn) {
                saveBtn.style.display = 'inline-flex';
            }

            this.setupModalForm(modal, controller, parentId, parentField);

            const bsModal = new bootstrap.Modal(modal, {
                backdrop: 'static',
                keyboard: true
            });

            bsModal.show();
            console.log('Modal exibido');

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });

        } catch (error) {
            console.error('Erro ao abrir modal de criação:', error);
            this.showToast('Erro ao abrir modal de criação', 'error');
        }
    }

    async openEditModal(controller, id, parentId) {
        try {
            console.log('Abrindo modal de edição:', { controller, id, parentId });

            const modal = this.createModalElement(controller, 'edit');
            document.body.appendChild(modal);

            const url = `/${controller}/Edit/${id}?modal=true`;
            console.log('URL de edição:', url);

            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao carregar formulário: ${response.status}`);
            }

            const html = await response.text();
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const saveBtn = modal.querySelector('#modalSaveBtn');
            if (saveBtn) {
                saveBtn.style.display = 'inline-flex';
            }

            const parentField = this.getParentFieldName(controller);
            this.setupModalForm(modal, controller, parentId, parentField);

            const bsModal = new bootstrap.Modal(modal, {
                backdrop: 'static',
                keyboard: true
            });

            bsModal.show();

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });

        } catch (error) {
            console.error('Erro ao abrir modal de edição:', error);
            this.showToast('Erro ao abrir modal de edição', 'error');
        }
    }

    getParentFieldName(controller) {
        // Mapeamento de controllers para nomes de campos
        const mapping = {
            'Despesas': 'IdVeiculo',
            'VeiculoDocumentos': 'IdVeiculo',
            'VeiculoFotos': 'IdVeiculo',
            'VeiculoNFE': 'IdVeiculo',
            'VeiculoLancamentos': 'IdVeiculo',
            // Adicione outros mapeamentos conforme necessário
        };

        return mapping[controller] || null;
    }

    createModalElement(controller, mode) {
        const modalId = `modal_${controller}_${mode}_${Date.now()}`;
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = modalId;
        modal.setAttribute('tabindex', '-1');
        modal.setAttribute('data-controller', controller);
        modal.setAttribute('data-mode', mode);

        modal.innerHTML = `
            <div class="modal-dialog modal-xl" style="max-width: 90%; width: 1400px;">
                <div class="modal-content" style="min-height: 85vh; max-height: 95vh; display: flex; flex-direction: column;">
                    <div class="modal-header bg-primary text-white" style="flex-shrink: 0;">
                        <h5 class="modal-title">
                            <i class="fas fa-${mode === 'create' ? 'plus' : 'edit'} me-2"></i>
                            ${mode === 'create' ? 'Adicionar' : 'Editar'} ${controller}
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0" style="flex: 1; overflow-y: auto;">
                        <div class="d-flex justify-content-center align-items-center py-5" style="min-height: 300px;">
                            <div class="text-center">
                                <div class="spinner-border text-primary" role="status">
                                    <span class="visually-hidden">Carregando...</span>
                                </div>
                                <p class="mt-3 text-muted">Carregando formulário...</p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer" style="flex-shrink: 0; background: #f8f9fa; border-top: 1px solid #dee2e6;">
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-2"></i>
                            Cancelar
                        </button>
                        <button type="button" class="btn btn-primary" id="modalSaveBtn" style="display: none;">
                            <i class="fas fa-save me-2"></i>
                            Salvar
                        </button>
                    </div>
                </div>
            </div>
        `;

        return modal;
    }

    setupModalForm(modal, controller, parentId, parentField) {
        const form = modal.querySelector('form');
        if (!form) {
            console.error('Formulário não encontrado no modal');
            return;
        }

        console.log('Configurando formulário do modal');

        // Inicializar campos do formulário
        if (typeof window.referenceFieldManager !== 'undefined') {
            window.referenceFieldManager.initializeFormElements(modal);
        }

        // Inicializar máscaras
        if (typeof $.fn.mask !== 'undefined') {
            const $modal = $(modal);
            $modal.find('.cpf-mask').mask('000.000.000-00');
            $modal.find('.cnpj-mask').mask('00.000.000/0000-00');
            $modal.find('.telefone-mask').mask('(00) 0000-00009');
            $modal.find('.cep-mask').mask('00000-000');
            $modal.find('.placa-mask').mask('AAA-0A00');
            $modal.find('.money-mask').mask('#.##0,00', { reverse: true });
        }

        const saveBtn = modal.querySelector('#modalSaveBtn');
        if (saveBtn) {
            saveBtn.onclick = async (e) => {
                e.preventDefault();
                await this.handleModalSubmit(modal, form, controller, parentId, parentField);
            };
        }
    }

    async handleModalSubmit(modal, form, controller, parentId, parentField) {
        const saveBtn = modal.querySelector('#modalSaveBtn');

        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            this.showToast('Por favor, preencha todos os campos obrigatórios', 'warning');
            return;
        }

        const originalBtnContent = saveBtn.innerHTML;
        saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Salvando...';
        saveBtn.disabled = true;

        try {
            const formData = new FormData(form);

            // CRÍTICO: Adicionar IdEmpresa do usuário logado
            const idEmpresaElement = document.querySelector('input[name="IdEmpresa"]') ||
                document.querySelector('[data-id-empresa]');

            if (idEmpresaElement) {
                const idEmpresa = idEmpresaElement.value || idEmpresaElement.dataset.idEmpresa;
                if (idEmpresa && !formData.has('IdEmpresa')) {
                    formData.set('IdEmpresa', idEmpresa);
                    console.log('IdEmpresa adicionado ao FormData:', idEmpresa);
                }
            }

            // Adicionar campo pai se fornecido
            if (parentId && parentField) {
                formData.set(parentField, parentId);
                console.log(`${parentField} adicionado ao FormData:`, parentId);
            }

            // Log de todos os dados do FormData
            console.log('=== DADOS ENVIADOS ===');
            for (let pair of formData.entries()) {
                console.log(pair[0] + ': ' + pair[1]);
            }
            console.log('======================');

            const response = await fetch(form.action, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: formData
            });

            if (response.ok) {
                const contentType = response.headers.get('content-type');

                if (contentType && contentType.includes('application/json')) {
                    const result = await response.json();

                    if (result.success) {
                        this.showToast(result.message || 'Registro salvo com sucesso!', 'success');

                        const bsModal = bootstrap.Modal.getInstance(modal);
                        if (bsModal) {
                            bsModal.hide();
                        }

                        // Recarregar a tab
                        await this.reloadTab(controller, parentId);
                    } else {
                        this.showValidationErrors(form, result.errors || {});
                    }
                } else {
                    // HTML de resposta - recarregar formulário com erros
                    const html = await response.text();
                    const modalBody = modal.querySelector('.modal-body');
                    modalBody.innerHTML = html;
                    this.setupModalForm(modal, controller, parentId, parentField);
                }
            } else {
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }

        } catch (error) {
            console.error('Erro ao salvar:', error);
            this.showToast(`Erro ao salvar: ${error.message}`, 'error');
        } finally {
            if (saveBtn) {
                saveBtn.innerHTML = originalBtnContent;
                saveBtn.disabled = false;
            }
        }
    }

    async reloadTab(controller, parentId) {
        const tabPane = document.querySelector('.tab-pane.active');
        if (!tabPane) return;

        const tabId = tabPane.id;
        await this.loadTabContent(tabId, controller, parentId);
    }

    async loadTabContent(tabId, controller, parentId) {
        const tabPane = document.getElementById(tabId);
        if (!tabPane) return;

        try {
            const parentField = this.getParentFieldName(controller);
            let url = `/${controller}?tab=${tabId}`;

            if (parentId && parentField) {
                url += `&${parentField}=${parentId}`;
            }

            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao carregar conteúdo: ${response.status}`);
            }

            const html = await response.text();
            tabPane.innerHTML = html;

        } catch (error) {
            console.error('Erro ao carregar tab:', error);
            tabPane.innerHTML = `
                <div class="alert alert-danger m-4">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    Erro ao carregar conteúdo. Por favor, recarregue a página.
                </div>
            `;
        }
    }

    async deleteItem(controller, id) {
        if (!confirm('Tem certeza que deseja excluir este registro?')) {
            return;
        }

        try {
            const response = await fetch(`/${controller}/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    this.showToast('Registro excluído com sucesso!', 'success');
                    await this.reloadTab(controller, null);
                } else {
                    this.showToast(result.message || 'Erro ao excluir registro', 'error');
                }
            } else {
                throw new Error(`Erro ${response.status}`);
            }

        } catch (error) {
            console.error('Erro ao excluir:', error);
            this.showToast('Erro ao excluir registro', 'error');
        }
    }

    showValidationErrors(form, errors) {
        form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
        form.querySelectorAll('.invalid-feedback').forEach(el => el.remove());

        for (const [field, messages] of Object.entries(errors)) {
            const input = form.querySelector(`[name="${field}"]`);
            if (input) {
                input.classList.add('is-invalid');
                const feedback = document.createElement('div');
                feedback.className = 'invalid-feedback d-block';
                feedback.textContent = Array.isArray(messages) ? messages[0] : messages;
                input.parentElement.appendChild(feedback);
            }
        }

        this.showToast('Por favor, corrija os erros no formulário', 'error');
    }

    showToast(message, type = 'info') {
        if (typeof Toastify !== 'undefined') {
            Toastify({
                text: message,
                duration: 3000,
                gravity: 'top',
                position: 'right',
                backgroundColor: type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#17a2b8',
            }).showToast();
        } else {
            alert(message);
        }
    }
}

// Inicializar
const tabContentManager = new TabContentManager();

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => tabContentManager.init());
} else {
    tabContentManager.init();
}

window.tabContentManager = tabContentManager;