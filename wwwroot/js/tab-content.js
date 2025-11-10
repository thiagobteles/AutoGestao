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
                e.stopPropagation();
                const btn = e.target.closest('.btn-add-tab-item');
                const tabId = btn.dataset.tab;
                const controller = btn.dataset.controller;
                const parentId = btn.dataset.parentId;
                const parentController = btn.dataset.parentController;

                console.log('Botão adicionar clicado:', { tabId, controller, parentId, parentController });

                this.openCreateModal(controller, parentId, parentController, tabId);
            }

            // Botão editar
            if (e.target.closest('.btn-edit-tab-item')) {
                e.preventDefault();
                e.stopPropagation();
                const btn = e.target.closest('.btn-edit-tab-item');
                const id = btn.dataset.id;
                const controller = btn.dataset.controller;
                const parentId = btn.dataset.parentId;
                const parentController = btn.dataset.parentController;

                console.log('Botão editar clicado:', { id, controller, parentId, parentController });

                // Validar que o ID é um número válido
                if (!id || isNaN(parseInt(id))) {
                    console.error('ID inválido:', id);
                    this.showError('Erro: ID do registro inválido');
                    return;
                }

                this.openEditModal(controller, parseInt(id), parentId, parentController);
            }

            // Botão deletar
            if (e.target.closest('.btn-delete-tab-item')) {
                e.preventDefault();
                e.stopPropagation();
                const btn = e.target.closest('.btn-delete-tab-item');
                const id = btn.dataset.id;
                const controller = btn.dataset.controller;

                console.log('Botão deletar clicado:', { id, controller });

                // Validar que o ID é um número válido
                if (!id || isNaN(parseInt(id))) {
                    console.error('ID inválido:', id);
                    this.showError('Erro: ID do registro inválido');
                    return;
                }

                this.deleteItem(controller, parseInt(id));
            }
        });
    }

    async openCreateModal(controller, parentId, parentController, tabId) {
        try {
            console.log('Abrindo modal de criação:', { controller, parentId, parentController, tabId });

            const modal = this.createModalElement(controller, 'create');
            document.body.appendChild(modal);

            const parentField = this.getParentFieldName(controller, parentController);
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
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const saveBtn = modal.querySelector('#modalSaveBtn');
            if (saveBtn) {
                saveBtn.style.display = 'inline-flex';
            }

            if (parentId && parentField) {
                this.lockParentField(modal, parentField, parentId, parentController);
            }

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
            showError('Erro ao abrir modal de criação: ' + error.message);
        }
    }

    async openEditModal(controller, id, parentId, parentController) {
        try {
            if (!id || typeof id !== 'number') {
                throw new Error(`ID inválido: ${id}`);
            }

            const modal = this.createModalElement(controller, 'edit');
            document.body.appendChild(modal);

            // URL correta: /Controller/Edit/ID?modal=true
            const url = `/${controller}/Edit/${id}?modal=true`;
            console.log('URL de edição:', url);

            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }

            const html = await response.text();
            const modalBody = modal.querySelector('.modal-body');
            modalBody.innerHTML = html;

            const saveBtn = modal.querySelector('#modalSaveBtn');
            if (saveBtn) {
                saveBtn.style.display = 'inline-flex';
            }

            const parentField = this.getParentFieldName(controller, parentController);
            if (parentId && parentField) {
                this.lockParentField(modal, parentField, parentId, parentController);
            }

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
            showError('Erro ao abrir modal de edição: ' + error.message);
        }
    }

    getParentFieldName(controller, parentController) {
        if (!parentController) {
            return null;
        }

        let singularParent = parentController;
        const fieldName = `Id${singularParent}`;
        return fieldName;
    }

    async lockParentField(modal, parentField, parentId, parentController) {
        console.log(`🔒 Tentando bloquear campo: ${parentField} com valor: ${parentId} (controller: ${parentController})`);

        // Aguardar um pouco mais para garantir que o HTML foi carregado
        await new Promise(resolve => setTimeout(resolve, 300));

        const hiddenInput = modal.querySelector(`input[name="${parentField}"]`);

        if (hiddenInput) {
            console.log(`✅ Campo hidden encontrado: ${parentField}`);
            hiddenInput.value = parentId;

            // Buscar o campo de pesquisa visual
            const searchInput = modal.querySelector(`input.reference-search-input[data-target-field="${parentField}"]`);

            if (searchInput) {
                console.log('✅ Campo de busca encontrado:', searchInput.id);

                // SEMPRE buscar o display name, independente se o campo já tem valor
                console.log(`📞 Buscando display name de ${parentController} #${parentId}...`);
                const displayName = await this.fetchParentDisplayName(parentController, parentId);

                if (displayName) {
                    console.log(`✅ Display name encontrado: ${displayName}`);
                    searchInput.value = displayName;
                    searchInput.classList.add('selected');
                } else {
                    console.warn(`⚠️ Não foi possível buscar o display name para ${parentController} #${parentId}`);
                    // Fallback: tentar buscar do próprio input se já tiver valor
                    if (!searchInput.value || searchInput.value === '') {
                        searchInput.value = `${parentController} #${parentId}`;
                        searchInput.classList.add('selected');
                    }
                }

                // Bloquear o campo
                searchInput.setAttribute('readonly', 'readonly');
                searchInput.setAttribute('disabled', 'disabled');
                const bgDisabled = getComputedStyle(document.documentElement).getPropertyValue('--bg-disabled').trim() || '#e9ecef';
                searchInput.style.backgroundColor = bgDisabled;
                searchInput.style.cursor = 'not-allowed';
                searchInput.title = 'Este campo não pode ser alterado pois o registro pertence a este contexto';

                console.log(`🔒 Campo ${parentField} bloqueado com sucesso`);
            } else {
                console.warn(`⚠️ Campo de busca não encontrado para: ${parentField}`);
            }

            // Esconder botões de ação
            const container = hiddenInput.closest('.reference-field-container');
            if (container) {
                const createBtn = container.querySelector('.reference-create-btn');
                if (createBtn) {
                    createBtn.style.display = 'none';
                }

                const clearBtn = container.querySelector('.reference-clear-btn');
                if (clearBtn) {
                    clearBtn.style.display = 'none';
                }

                const searchBtn = container.querySelector('.reference-search-btn');
                if (searchBtn) {
                    searchBtn.style.display = 'none';
                }
            }

            // Adicionar ícone de cadeado
            if (searchInput && searchInput.parentElement) {
                const lockIcon = document.createElement('span');
                lockIcon.className = 'position-absolute field-lock-icon';
                const textMuted = getComputedStyle(document.documentElement).getPropertyValue('--text-muted').trim() || '#6c757d';
                lockIcon.style.cssText = `right: 10px; top: 50%; transform: translateY(-50%); pointer-events: none; color: ${textMuted};`;
                lockIcon.innerHTML = '<i class="fas fa-lock"></i>';

                const parent = searchInput.parentElement;
                if (parent.style.position !== 'relative' && parent.style.position !== 'absolute') {
                    parent.style.position = 'relative';
                }
                parent.appendChild(lockIcon);
            }
        } else {
            console.error(`❌ Campo ${parentField} não encontrado no formulário`);
        }
    }

    async fetchParentDisplayName(parentController, parentId) {
        try {
            console.log(`📞 Fazendo requisição para buscar display name: ${parentController} #${parentId}`);

            const response = await fetch(`/api/Reference/GetById`, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    entityType: parentController,
                    id: parentId.toString()
                })
            });

            if (response.ok) {
                const result = await response.json();
                console.log(`✅ Display name recebido:`, result);
                return result.text || result.displayName || result.name || `#${parentId}`;
            } else {
                const errorText = await response.text();
                console.warn(`⚠️ Erro ao buscar display name: ${response.status} ${response.statusText}`, errorText);
            }
        } catch (error) {
            console.warn('❌ Erro ao buscar o nome do registro pai:', error);
        }

        return null;
    }

    createModalElement(controller, mode) {
        const modalId = `modal_${controller}_${mode}_${Date.now()}`;
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = modalId;
        modal.setAttribute('tabindex', '-1');
        modal.setAttribute('data-controller', controller);
        modal.setAttribute('data-mode', mode);
        modal.setAttribute('data-modal-level', '1'); // IMPORTANTE: Define nível do modal para que campos de referência possam abrir modais aninhados corretamente

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
                    <div class="modal-footer" style="flex-shrink: 0; background: var(--light-bg); border-top: 1px solid var(--border-light);">
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

        if (typeof window.referenceFieldManager !== 'undefined') {
            window.referenceFieldManager.initializeAllFields(modal);
        }

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
            this.showWarning('Por favor, preencha todos os campos obrigatórios');
            return;
        }

        const originalBtnContent = saveBtn.innerHTML;
        saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Salvando...';
        saveBtn.disabled = true;

        try {
            const formData = new FormData(form);

            const idEmpresaElement = document.querySelector('input[name="IdEmpresa"]') ||
                document.querySelector('[data-id-empresa]');

            if (idEmpresaElement) {
                const idEmpresa = idEmpresaElement.value || idEmpresaElement.dataset.idEmpresa;
                if (idEmpresa && !formData.has('IdEmpresa')) {
                    formData.set('IdEmpresa', idEmpresa);
                    console.log('IdEmpresa adicionado ao FormData:', idEmpresa);
                }
            }

            if (parentId && parentField) {
                formData.set(parentField, parentId);
                console.log(`${parentField} adicionado/atualizado no FormData:`, parentId);
            }

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
                        showSuccess(result.message || 'Registro salvo com sucesso!');

                        const bsModal = bootstrap.Modal.getInstance(modal);
                        if (bsModal) {
                            bsModal.hide();
                        }

                        // Recarregar a tab ativa
                        if (typeof window.tabSystem !== 'undefined') {
                            window.tabSystem.reloadActiveTab();
                        }
                    } else {
                        this.showValidationErrors(form, result.errors || {});
                    }
                } else {
                    const html = await response.text();
                    const modalBody = modal.querySelector('.modal-body');
                    modalBody.innerHTML = html;
                    this.setupModalForm(modal, controller, parentId, parentField);
                }
            } else {
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }

        } catch (error) {
            showError(`Erro ao salvar: ${error.message}`);
        } finally {
            if (saveBtn) {
                saveBtn.innerHTML = originalBtnContent;
                saveBtn.disabled = false;
            }
        }
    }

    async deleteItem(controller, id) {
        const confirmed = await confirmDelete(); if (!confirmed) {
            return;
        }

        try {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenInput ? tokenInput.value : '';

            const response = await fetch(`/${controller}/Delete/${id}`, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: token ? `__RequestVerificationToken=${encodeURIComponent(token)}` : ''
            });

            if (response.ok) {
                const result = await response.json();

                if (result.success) {
                    this.showSuccess('Registro excluído com sucesso!');

                    if (typeof window.tabSystem !== 'undefined') {
                        window.tabSystem.reloadActiveTab();
                    }
                } else {
                    showError(result.message || 'Erro ao excluir registro');
                }
            } else {
                throw new Error(`Erro ${response.status}`);
            }

        } catch (error) {
            console.error('Erro ao excluir:', error);
            this.showError('Erro ao excluir registro');
        }
    }

    // Métodos helper para alertas (delegam para funções globais)
    showError(message) {
        if (typeof window.showError === 'function') {
            window.showError(message);
        }
    }

    showSuccess(message) {
        if (typeof window.showSuccess === 'function') {
            window.showSuccess(message);
        }
    }

    showWarning(message) {
        if (typeof window.showWarning === 'function') {
            window.showWarning(message);
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

        this.showError('Por favor, corrija os erros no formulário');
    }
}

const tabContentManager = new TabContentManager();

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => tabContentManager.init());
} else {
    tabContentManager.init();
}

window.tabContentManager = tabContentManager;