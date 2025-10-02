class TabbedFormManager {
    constructor() {
        this.currentEntityId = null;
        this.loadedTabs = new Set();
        this.tabCache = new Map();

        this.init();
    }

    init() {
        this.initializeTabs();
        this.initializeModalForms();
        this.initializeFileUploads();
        this.setupEventListeners();

        console.log('Sistema de abas inicializado');
    }

    // ================================================================================================
    // GERENCIAMENTO DE ABAS
    // ================================================================================================

    initializeTabs() {
        // Interceptar cliques nas abas
        document.querySelectorAll('#entityTabs button[data-bs-toggle="tab"]').forEach(tabButton => {
            tabButton.addEventListener('shown.bs.tab', (e) => {
                this.handleTabShown(e);
            });
        });

        // Detectar ID da entidade
        const form = document.querySelector('.standard-form');
        if (form) {
            const idInput = form.querySelector('input[name="Id"]');
            this.currentEntityId = idInput ? idInput.value : 0;
        }

        // Marcar aba principal como carregada
        this.loadedTabs.add('principal');
    }

    async handleTabShown(event) {
        const tabId = event.target.dataset.tabId;
        const lazyLoad = event.target.dataset.lazyLoad === 'true';
        const controller = event.target.dataset.controller;
        const action = event.target.dataset.action || 'Index';

        // Se não precisa de lazy loading ou já foi carregada, não fazer nada
        if (!lazyLoad || this.loadedTabs.has(tabId)) {
            return;
        }

        // Verificar se tem cache
        if (this.tabCache.has(tabId)) {
            this.renderCachedTab(tabId);
            return;
        }

        // Carregar conteúdo da aba
        await this.loadTabContent(tabId, controller, action);
    }

    async loadTabContent(tabId, controller, action) {
        const tabContent = document.querySelector(`#${tabId}-content .tab-content-wrapper`);
        const tabButton = document.querySelector(`#${tabId}-tab`);

        if (!tabContent) return;

        try {
            // Mostrar loading no botão da aba
            this.setTabLoading(tabButton, true);

            // Mostrar loading no conteúdo
            tabContent.innerHTML = this.getLoadingHtml();

            // Fazer requisição AJAX
            const url = `/${this.getControllerName()}/RenderTab?id=${this.currentEntityId}&tabId=${tabId}`;
            const response = await fetch(url);

            if (response.ok) {
                const html = await response.text();
                tabContent.innerHTML = html;

                // Marcar como carregada e cachear
                this.loadedTabs.add(tabId);
                this.tabCache.set(tabId, html);

                // Inicializar funcionalidades da aba
                this.initializeTabFeatures(tabId);

            } else {
                throw new Error(`Erro ${response.status}: ${response.statusText}`);
            }

        } catch (error) {
            console.error('Erro ao carregar aba:', error);
            tabContent.innerHTML = this.getErrorHtml(error.message);
        } finally {
            this.setTabLoading(tabButton, false);
        }
    }

    renderCachedTab(tabId) {
        const tabContent = document.querySelector(`#${tabId}-content .tab-content-wrapper`);
        const cachedHtml = this.tabCache.get(tabId);

        if (tabContent && cachedHtml) {
            tabContent.innerHTML = cachedHtml;
            this.initializeTabFeatures(tabId);
        }
    }

    setTabLoading(tabButton, loading) {
        const loadingSpinner = tabButton.querySelector('.tab-loading');
        if (loadingSpinner) {
            if (loading) {
                loadingSpinner.classList.remove('d-none');
            } else {
                loadingSpinner.classList.add('d-none');
            }
        }
    }

    initializeTabFeatures(tabId) {
        const tabContent = document.querySelector(`#${tabId}-content`);
        if (!tabContent) return;

        // Reinicializar máscaras e validações
        if (window.initializeStandardForm) {
            window.initializeStandardForm();
        }

        // Inicializar funcionalidades específicas por aba
        switch (tabId) {
            case 'arquivos':
                this.initializeArquivosTab(tabContent);
                break;
            case 'midias':
                this.initializeMidiasTab(tabContent);
                break;
            case 'financeiro':
                this.initializeFinanceiroTab(tabContent);
                break;
        }
    }

    // ================================================================================================
    // FUNCIONALIDADES ESPECÍFICAS POR ABA
    // ================================================================================================

    initializeArquivosTab(tabContent) {
        // Upload de documentos
        const uploadForm = tabContent.querySelector('#uploadDocumentoForm');
        if (uploadForm) {
            uploadForm.addEventListener('submit', (e) => this.handleDocumentoUpload(e));
        }
    }

    initializeMidiasTab(tabContent) {
        // Upload de fotos
        const uploadForm = tabContent.querySelector('#uploadFotoForm');
        if (uploadForm) {
            uploadForm.addEventListener('submit', (e) => this.handleFotoUpload(e));
        }

        // Visualização de fotos
        this.initializeFotoGallery(tabContent);
    }

    initializeFinanceiroTab(tabContent) {
        // Formulário de despesas
        const despesaForm = tabContent.querySelector('#novaDespesaForm');
        if (despesaForm) {
            despesaForm.addEventListener('submit', (e) => this.handleDespesaSubmit(e));
        }

        // Carregar fornecedores
        this.loadFornecedores(tabContent);
    }

    // ================================================================================================
    // UPLOAD DE ARQUIVOS
    // ================================================================================================

    initializeFileUploads() {
        // Configurar dropzone para arquivos se necessário
        this.setupDropzones();
    }

    async handleDocumentoUpload(event) {
        event.preventDefault();

        const form = event.target;
        const formData = new FormData(form);
        formData.append('idVeiculo', this.currentEntityId);

        try {
            this.setFormLoading(form, true);

            const response = await fetch(`/${this.getControllerName()}/AdicionarDocumento`, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.closeModal(form.closest('.modal'));
                this.refreshTab('arquivos');
                form.reset();
            } else {
                this.showToast(result.message || 'Erro ao fazer upload', 'error');
            }

        } catch (error) {
            console.error('Erro no upload:', error);
            this.showToast('Erro ao fazer upload do documento', 'error');
        } finally {
            this.setFormLoading(form, false);
        }
    }

    async handleFotoUpload(event) {
        event.preventDefault();

        const form = event.target;
        const formData = new FormData(form);
        formData.append('idVeiculo', this.currentEntityId);

        try {
            this.setFormLoading(form, true);

            const response = await fetch(`/${this.getControllerName()}/AdicionarFoto`, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.closeModal(form.closest('.modal'));
                this.refreshTab('midias');
                form.reset();
            } else {
                this.showToast(result.message || 'Erro ao fazer upload', 'error');
            }

        } catch (error) {
            console.error('Erro no upload:', error);
            this.showToast('Erro ao fazer upload das fotos', 'error');
        } finally {
            this.setFormLoading(form, false);
        }
    }

    // ================================================================================================
    // FORMULÁRIOS MODAIS
    // ================================================================================================

    initializeModalForms() {
        // Inicializar modais quando necessário
        document.addEventListener('shown.bs.modal', (e) => {
            const modal = e.target;
            this.initializeModalContent(modal);
        });
    }

    initializeModalContent(modal) {
        // Aplicar máscaras nos campos do modal
        const currencyInputs = modal.querySelectorAll('.currency-mask');
        currencyInputs.forEach(input => {
            this.applyCurrencyMask(input);
        });

        // Focar no primeiro campo
        const firstInput = modal.querySelector('input, select, textarea');
        if (firstInput) {
            setTimeout(() => firstInput.focus(), 100);
        }
    }

    async handleDespesaSubmit(event) {
        event.preventDefault();

        const form = event.target;
        const formData = new FormData(form);
        formData.append('idVeiculo', this.currentEntityId);

        try {
            this.setFormLoading(form, true);

            const response = await fetch(`/${this.getControllerName()}/AdicionarDespesa`, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.closeModal(form.closest('.modal'));
                this.refreshTab('financeiro');
                form.reset();
            } else {
                this.showToast(result.message || 'Erro ao salvar despesa', 'error');
            }

        } catch (error) {
            console.error('Erro ao salvar despesa:', error);
            this.showToast('Erro ao salvar despesa', 'error');
        } finally {
            this.setFormLoading(form, false);
        }
    }

    // ================================================================================================
    // OPERAÇÕES CRUD
    // ================================================================================================

    async removerDocumento(documentoId) {
        if (!confirm('Tem certeza que deseja remover este documento?')) {
            return;
        }

        try {
            const response = await fetch(`/${this.getControllerName()}/RemoverDocumento`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ documentoId })
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.refreshTab('arquivos');
            } else {
                this.showToast(result.message || 'Erro ao remover documento', 'error');
            }

        } catch (error) {
            console.error('Erro ao remover documento:', error);
            this.showToast('Erro ao remover documento', 'error');
        }
    }

    async removerFoto(fotoId) {
        if (!confirm('Tem certeza que deseja remover esta foto?')) {
            return;
        }

        try {
            const response = await fetch(`/${this.getControllerName()}/RemoverFoto`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ fotoId })
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.refreshTab('midias');
            } else {
                this.showToast(result.message || 'Erro ao remover foto', 'error');
            }

        } catch (error) {
            console.error('Erro ao remover foto:', error);
            this.showToast('Erro ao remover foto', 'error');
        }
    }

    async removerDespesa(despesaId) {
        if (!confirm('Tem certeza que deseja remover esta despesa?')) {
            return;
        }

        try {
            const response = await fetch(`/Despesas/Delete/${despesaId}`, {
                method: 'POST'
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.refreshTab('financeiro');
            } else {
                this.showToast(result.message || 'Erro ao remover despesa', 'error');
            }

        } catch (error) {
            console.error('Erro ao remover despesa:', error);
            this.showToast('Erro ao remover despesa', 'error');
        }
    }

    // ================================================================================================
    // UTILITÁRIOS
    // ================================================================================================

    async refreshTab(tabId) {
        // Remover do cache e recarregar
        this.tabCache.delete(tabId);
        this.loadedTabs.delete(tabId);

        // Se a aba está ativa, recarregar imediatamente
        const tabButton = document.querySelector(`#${tabId}-tab`);
        if (tabButton && tabButton.classList.contains('active')) {
            const controller = tabButton.dataset.controller;
            const action = tabButton.dataset.action || 'Index';
            await this.loadTabContent(tabId, controller, action);
        }
    }

    setupEventListeners() {
        // Expor funções globalmente para uso nos templates
        window.removerDocumento = (id) => this.removerDocumento(id);
        window.removerFoto = (id) => this.removerFoto(id);
        window.removerDespesa = (id) => this.removerDespesa(id);
        window.abrirFotoModal = (url, descricao) => this.abrirFotoModal(url, descricao);
        window.editarDespesa = (id) => this.editarDespesa(id);
    }

    abrirFotoModal(url, descricao) {
        const modal = document.querySelector('#fotoViewModal');
        const img = modal.querySelector('#fotoViewImage');
        const title = modal.querySelector('#fotoViewTitle');

        img.src = url;
        img.alt = descricao;
        title.textContent = descricao || 'Visualizar Foto';

        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
    }

    editarDespesa(despesaId) {
        // Implementar edição de despesa
        console.log('Editar despesa:', despesaId);
    }

    async loadFornecedores(tabContent) {
        const select = tabContent.querySelector('select[name="IdFornecedor"]');
        if (!select) return;

        try {
            const response = await fetch('/Fornecedores/GetSelectOptions');
            const fornecedores = await response.json();

            fornecedores.forEach(fornecedor => {
                const option = document.createElement('option');
                option.value = fornecedor.value;
                option.textContent = fornecedor.text;
                select.appendChild(option);
            });

        } catch (error) {
            console.error('Erro ao carregar fornecedores:', error);
        }
    }

    initializeFotoGallery(tabContent) {
        // Implementar galeria de fotos se necessário
        const fotos = tabContent.querySelectorAll('.foto-thumbnail');
        fotos.forEach(foto => {
            foto.addEventListener('click', () => {
                const url = foto.src;
                const descricao = foto.alt;
                this.abrirFotoModal(url, descricao);
            });
        });
    }

    setupDropzones() {
        // Implementar drag & drop para upload de arquivos
        document.querySelectorAll('.dropzone').forEach(zone => {
            this.initializeDropzone(zone);
        });
    }

    // ================================================================================================
    // HELPERS
    // ================================================================================================

    getControllerName() {
        // Extrair nome do controller da URL atual
        const path = window.location.pathname;
        const segments = path.split('/').filter(s => s);
        return segments[0] || 'Veiculos';
    }

    setFormLoading(form, loading) {
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) {
            submitBtn.disabled = loading;
            if (loading) {
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processando...';
            } else {
                // Restaurar texto original (pode ser melhorado)
                const icon = submitBtn.dataset.icon || 'fas fa-save';
                const text = submitBtn.dataset.text || 'Salvar';
                submitBtn.innerHTML = `<i class="${icon}"></i> ${text}`;
            }
        }
    }

    closeModal(modal) {
        if (modal) {
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) {
                bsModal.hide();
            }
        }
    }

    applyCurrencyMask(input) {
        // Formatar valor inicial se já existir
        formatCurrencyValueOnLoad(input);

        input.addEventListener('input', function () {
            let value = this.value.replace(/\D/g, '');
            if (value === '') {
                this.value = '';
                return;
            }
            value = (parseInt(value) / 100).toFixed(2);
            value = value.replace('.', ',');
            value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
            this.value = 'R$ ' + value;
        });

        input.addEventListener('focus', function () {
            let value = this.value.replace(/[^\d,]/g, '');
            if (value) {
                this.value = value.replace(',', '.');
            }
        });

        input.addEventListener('blur', function () {
            formatCurrencyValueOnLoad(this);
        });
    }

    showToast(message, type = 'info') {
        // Usar sistema de toast existente ou criar um simples
        if (window.showToast) {
            window.showToast(message, type);
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }

    getLoadingHtml() {
        return `
            <div class="d-flex justify-content-center align-items-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
                <span class="ms-3">Carregando conteúdo...</span>
            </div>
        `;
    }

    getErrorHtml(message) {
        return `
            <div class="alert alert-danger text-center py-5">
                <i class="fas fa-exclamation-triangle fa-3x mb-3"></i>
                <h5>Erro ao carregar conteúdo</h5>
                <p>${message}</p>
                <button class="btn btn-outline-danger" onclick="location.reload()">
                    <i class="fas fa-redo"></i>
                    Tentar Novamente
                </button>
            </div>
        `;
    }
}

// ================================================================================================
// INICIALIZAÇÃO
// ================================================================================================

function formatCurrencyValueOnLoad(input) {
    if (!input.value || input.value.trim() === '') {
        return;
    }

    // Se já está formatado, não fazer nada
    if (input.value.startsWith('R$')) {
        return;
    }

    // Tentar parsear o valor
    let value = input.value.replace(/[^\d,.]/g, '').replace(',', '.');
    let numericValue = parseFloat(value);

    if (!isNaN(numericValue)) {
        // Formatar como moeda brasileira
        let formatted = numericValue.toFixed(2);
        formatted = formatted.replace('.', ',');
        formatted = formatted.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
        input.value = 'R$ ' + formatted;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Verificar se é uma página com abas
    if (document.querySelector('#entityTabs')) {
        window.tabbedFormManager = new TabbedFormManager();
    }
});