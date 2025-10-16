class StandardGrid {
    constructor(options = {}) {
        this.options = {
            loadingSelector: '#loadingOverlay',
            gridContainerSelector: '#gridContainer',
            filtersFormSelector: '#filtrosForm',
            pageSizeSelector: '#pageSizeSelector',
            searchDebounceTime: 600,
            ...options
        };

        this.isLoading = false;
        this.loadingTimeout = null;
        this.searchTimeouts = new Map();

        this.init();
    }

    init() {
        console.log('🚀 Inicializando Sistema de Grid Unificado...');

        this.setupLoadingProtection();
        this.setupFilters();
        this.setupEventListeners();
        this.setupAccessibility();
        this.forceHideLoading();

        console.log('✅ Sistema de Grid inicializado com sucesso');
    }

    // ===================================================================
    // SISTEMA DE LOADING
    // ===================================================================

    showLoading(show = true) {
        const overlay = document.querySelector(this.options.loadingSelector);

        if (!overlay) {
            console.warn('Loading overlay não encontrado');
            return;
        }

        if (this.loadingTimeout) {
            clearTimeout(this.loadingTimeout);
            this.loadingTimeout = null;
        }

        if (show) {
            this.isLoading = true;
            overlay.classList.remove('d-none');
            overlay.style.display = 'flex';
            overlay.style.opacity = '1';
            overlay.style.visibility = 'visible';

            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.add('loading');
            }

            // Timeout de segurança
            this.loadingTimeout = setTimeout(() => {
                console.warn('Loading forçado a esconder após timeout');
                this.showLoading(false);
            }, 15000);

        } else {
            this.isLoading = false;
            overlay.style.opacity = '0';

            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.remove('loading');
            }

            setTimeout(() => {
                overlay.classList.add('d-none');
                overlay.style.display = 'none';
                overlay.style.visibility = 'hidden';
            }, 300);
        }
    }

    forceHideLoading() {
        const overlay = document.querySelector(this.options.loadingSelector);
        if (overlay) {
            this.isLoading = false;
            overlay.classList.add('d-none');
            overlay.style.display = 'none';
            overlay.style.opacity = '0';
            overlay.style.visibility = 'hidden';

            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.remove('loading');
            }

            if (this.loadingTimeout) {
                clearTimeout(this.loadingTimeout);
                this.loadingTimeout = null;
            }
        }
    }

    setupLoadingProtection() {
        // Esconder loading inicial
        setTimeout(() => this.forceHideLoading(), 1000);

        // Event listener para quando a página termina de carregar
        window.addEventListener('load', () => {
            setTimeout(() => this.forceHideLoading(), 1500);
        });

        // Prevenir loading infinito em caso de erro
        window.addEventListener('error', (event) => {
            console.error('Erro JavaScript detectado:', event.error);
            this.forceHideLoading();
        });

        // Detectar mudanças de visibilidade da página
        document.addEventListener('visibilitychange', () => {
            if (document.visibilityState === 'visible' && this.isLoading) {
                setTimeout(() => {
                    if (this.isLoading) {
                        console.warn('Loading ainda visível após mudança de visibilidade, forçando esconder');
                        this.forceHideLoading();
                    }
                }, 3000);
            }
        });
    }

    // ===================================================================
    // SISTEMA DE FILTROS
    // ===================================================================

    setupFilters() {
        const form = document.querySelector(this.options.filtersFormSelector);
        if (!form) return;

        // Filtros de texto com debounce
        const textInputs = form.querySelectorAll('input[type="text"], input[type="search"]');
        textInputs.forEach(input => {
            input.addEventListener('input', (e) => {
                this.handleTextFilterWithDebounce(e.target);
            });
        });

        // Filtros de seleção (aplicam imediatamente)
        const selectInputs = form.querySelectorAll('select');
        selectInputs.forEach(select => {
            select.addEventListener('change', () => {
                this.aplicarFiltros(1);
            });
        });

        // Filtros numéricos com debounce
        const numberInputs = form.querySelectorAll('input[type="number"]');
        numberInputs.forEach(input => {
            input.addEventListener('input', (e) => {
                this.handleTextFilterWithDebounce(e.target);
            });
        });

        // Filtros de data (aplicam imediatamente)
        const dateInputs = form.querySelectorAll('input[type="date"]');
        dateInputs.forEach(input => {
            input.addEventListener('change', () => {
                this.aplicarFiltros(1);
            });
        });

        // Seletor de tamanho de página
        const pageSizeSelector = document.querySelector(this.options.pageSizeSelector);
        if (pageSizeSelector) {
            pageSizeSelector.addEventListener('change', () => {
                this.aplicarFiltros(1);
            });
        }
    }

    handleTextFilterWithDebounce(input) {
        const filterId = input.name || input.id;

        // Limpar timeout anterior para este filtro específico
        if (this.searchTimeouts.has(filterId)) {
            clearTimeout(this.searchTimeouts.get(filterId));
        }

        // Configurar novo timeout
        const timeout = setTimeout(() => {
            this.aplicarFiltros(1);
            this.searchTimeouts.delete(filterId);
        }, this.options.searchDebounceTime);

        this.searchTimeouts.set(filterId, timeout);
    }

    aplicarFiltros(page = 1) {
        if (this.isLoading) {
            console.log('Já carregando, ignorando nova requisição');
            return;
        }

        this.showLoading(true);

        const form = document.querySelector(this.options.filtersFormSelector);
        if (!form) {
            this.showLoading(false);
            return;
        }

        const formData = new FormData(form);
        const pageSize = document.querySelector(this.options.pageSizeSelector)?.value || 50;

        const params = new URLSearchParams();
        params.append('page', page);
        params.append('pageSize', pageSize);

        // Adicionar filtros
        for (let [key, value] of formData.entries()) {
            if (value && value.trim() !== '') {
                params.append(key, value);
            }
        }

        // Determinar URL baseado na página atual
        const ajaxUrl = this.getAjaxUrl();
        if (!ajaxUrl) {
            this.forceHideLoading();
            return;
        }

        fetch(`${ajaxUrl}?${params.toString()}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                const gridContainer = document.querySelector(this.options.gridContainerSelector);
                if (gridContainer) {
                    gridContainer.innerHTML = html;
                    this.updateUrl(params);
                    this.reinitializeEvents();
                } else {
                    console.error('Grid container não encontrado');
                }
            })
            .catch(error => {
                console.error('Erro ao carregar dados:', error);
                this.showToast('Erro ao carregar dados: ' + error.message, 'error');
            })
            .finally(() => {
                setTimeout(() => {
                    this.showLoading(false);
                }, 300);
            });
    }

    getAjaxUrl() {
        const currentPath = window.location.pathname.toLowerCase();

        if (currentPath.includes('clientes')) {
            return '/Clientes/GetDataAjax';
        } else if (currentPath.includes('veiculos')) {
            return '/Veiculos/GetDataAjax';
        } else if (currentPath.includes('vendedores')) {
            return '/Vendedores/GetDataAjax';
        } else if (currentPath.includes('fornecedores')) {
            return '/Fornecedores/GetDataAjax';
        }

        console.error('URL Ajax não identificada para:', currentPath);
        return null;
    }

    limparFiltros() {
        const form = document.querySelector(this.options.filtersFormSelector);
        if (form) {
            form.reset();

            // Limpar todos os timeouts de pesquisa
            this.searchTimeouts.forEach(timeout => clearTimeout(timeout));
            this.searchTimeouts.clear();
        }

        const pageSize = document.querySelector(this.options.pageSizeSelector);
        if (pageSize) {
            pageSize.value = '50';
        }

        this.aplicarFiltros(1);
    }

    // ===================================================================
    // SISTEMA DE PAGINAÇÃO E ORDENAÇÃO
    // ===================================================================

    changePage(page) {
        this.aplicarFiltros(page);
    }

    sortColumn(column) {
        const orderByInput = document.querySelector('#orderBy') || this.createHiddenInput('orderBy', 'Nome');
        const orderDirectionInput = document.querySelector('#orderDirection') || this.createHiddenInput('orderDirection', 'asc');

        const currentOrder = orderByInput.value;
        const currentDirection = orderDirectionInput.value;

        if (currentOrder === column) {
            orderDirectionInput.value = currentDirection === 'asc' ? 'desc' : 'asc';
        } else {
            orderByInput.value = column;
            orderDirectionInput.value = 'asc';
        }

        this.aplicarFiltros();
    }

    createHiddenInput(name, defaultValue) {
        const form = document.querySelector(this.options.filtersFormSelector);
        if (!form) return null;

        const input = document.createElement('input');
        input.type = 'hidden';
        input.id = name;
        input.name = name;
        input.value = defaultValue;
        form.appendChild(input);

        return input;
    }

    // ===================================================================
    // SISTEMA DE EVENTOS
    // ===================================================================

    setupEventListeners() {
        this.reinitializeEvents();
    }

    reinitializeEvents() {
        // Duplo clique nas linhas
        const rows = document.querySelectorAll('.grid-row');
        rows.forEach(row => {
            row.removeEventListener('dblclick', this.handleRowDoubleClick);
            row.addEventListener('dblclick', this.handleRowDoubleClick.bind(this));
        });

        // Ordenação das colunas
        const sortableHeaders = document.querySelectorAll('.sortable-header');
        sortableHeaders.forEach(header => {
            header.removeEventListener('click', this.handleSortClick);
            header.addEventListener('click', this.handleSortClick.bind(this));

            // Suporte a teclado
            header.removeEventListener('keydown', this.handleSortKeydown);
            header.addEventListener('keydown', this.handleSortKeydown.bind(this));
        });

        // Navegação por teclado nas linhas
        rows.forEach((row, index) => {
            row.removeEventListener('keydown', this.handleRowKeydown);
            row.addEventListener('keydown', this.handleRowKeydown.bind(this));
        });

        // Disparar evento customizado
        document.dispatchEvent(new CustomEvent('gridUpdated'));
    }

    handleRowDoubleClick(e) {
        if (e.target.closest('.dropdown') || e.target.closest('.actions-btn')) {
            return;
        }

        const row = e.currentTarget;
        const id = row.getAttribute('data-id');
        const controller = row.getAttribute('data-controller');

        if (id && controller) {
            this.showLoading(true);
            window.location.href = `/${controller}/Details/${id}`;
        }
    }

    handleSortClick(e) {
        e.stopPropagation();
        const sortKey = e.currentTarget.getAttribute('data-sortable');
        if (sortKey) {
            this.sortColumn(sortKey);
        }
    }

    handleSortKeydown(e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            const sortKey = e.currentTarget.getAttribute('data-sortable');
            if (sortKey) {
                this.sortColumn(sortKey);
            }
        }
    }

    handleRowKeydown(e) {
        if (e.key === 'Enter') {
            e.currentTarget.dispatchEvent(new MouseEvent('dblclick'));
        }
    }

    // ===================================================================
    // ACESSIBILIDADE
    // ===================================================================

    setupAccessibility() {
        const table = document.querySelector('.base-grid-table');
        if (table) {
            table.setAttribute('role', 'grid');
            table.setAttribute('aria-label', 'Tabela de dados com filtros e ordenação');
        }

        const sortableHeaders = document.querySelectorAll('.sortable-header');
        sortableHeaders.forEach(header => {
            header.setAttribute('role', 'columnheader');
            header.setAttribute('tabindex', '0');
            header.setAttribute('aria-sort', 'none');
        });

        const rows = document.querySelectorAll('.grid-row');
        rows.forEach((row, index) => {
            row.setAttribute('role', 'row');
            row.setAttribute('tabindex', '0');
            row.setAttribute('aria-rowindex', index + 2);
        });
    }

    // ===================================================================
    // UTILITÁRIOS
    // ===================================================================

    updateUrl(params) {
        try {
            const newUrl = `${window.location.pathname}?${params.toString()}`;
            window.history.replaceState({}, '', newUrl);
        } catch (error) {
            console.error('Erro ao atualizar URL:', error);
        }
    }

    showToast(message, type = 'info', duration = 5000) {
        if (typeof window.showToast === 'function') {
            window.showToast(message, type, duration);
        } else {
            console.log(`Toast [${type}]: ${message}`);
        }
    }

    debug() {
        const overlay = document.querySelector(this.options.loadingSelector);
        const table = document.querySelector('.base-grid-table');
        const rows = document.querySelectorAll('.grid-row');

        console.log('=== GRID DEBUG ===');
        console.log('Loading overlay:', {
            exists: !!overlay,
            visible: this.isLoading,
            classes: overlay?.className,
            style: overlay?.style.cssText
        });
        console.log('Table:', {
            exists: !!table,
            classes: table?.className,
            rows: rows.length
        });
        console.log('Grid state:', {
            isLoading: this.isLoading,
            hasTimeout: !!this.loadingTimeout,
            searchTimeouts: this.searchTimeouts.size
        });
    }
}

// ===================================================================
// FUNÇÕES GLOBAIS PARA COMPATIBILIDADE
// ===================================================================

let gridInstance;

// Inicializar quando DOM estiver pronto
document.addEventListener('DOMContentLoaded', function () {
    gridInstance = new StandardGrid();

    // Exposer funções globais para compatibilidade
    window.aplicarFiltros = (page) => gridInstance.aplicarFiltros(page);
    window.changePage = (page) => gridInstance.changePage(page);
    window.limparFiltros = () => gridInstance.limparFiltros();
    window.sortColumn = (column) => gridInstance.sortColumn(column);
    window.showLoading = (show) => gridInstance.showLoading(show);
    window.forceHideLoading = () => gridInstance.forceHideLoading();
    window.debugGrid = () => gridInstance.debug();
});

// ===================================================================
// FUNÇÕES ESPECÍFICAS DE AÇÕES
// ===================================================================

window.confirmarExclusao = function (id) {
    // Usar modal de confirmação padrão do sistema, depois modal de resultado
    if (confirm('Tem certeza que deseja excluir este registro? Esta ação não pode ser desfeita.')) {
        const currentPath = window.location.pathname.toLowerCase();
        let controller = '';

        if (currentPath.includes('veiculos')) {
            controller = 'Veiculos';
        } else if (currentPath.includes('clientes')) {
            controller = 'Clientes';
        } else if (currentPath.includes('vendedores')) {
            controller = 'Vendedores';
        }

        if (controller) {
            window.showLoading(true);

            // Simular exclusão e mostrar resultado
            fetch(`/${controller}/Delete/${id}`, {
                method: 'POST'
            })
                .then(response => response.json())
                .then(result => {
                    window.showLoading(false);

                    if (result.sucesso) {
                        showSuccess('Registro excluído com sucesso!', {
                            title: 'Exclusão Realizada!',
                            buttonText: 'OK'
                        }).then(() => {
                            // Recarregar grid após fechar modal
                            location.reload();
                        });
                    } else {
                        showError('Não foi possível excluir o registro. ' + (result.mensagem || ''), {
                            title: 'Erro na Exclusão',
                            buttonText: 'Entendi'
                        });
                    }
                })
                .catch(error => {
                    window.showLoading(false);
                    showError('Erro de conexão durante a exclusão.', {
                        title: 'Erro de Sistema',
                        buttonText: 'Tentar Novamente'
                    });
                });
        }
    }
};

// Ações específicas para diferentes entidades
window.novaVenda = function (clienteId) {
    window.showLoading(true);
    if (clienteId) {
        window.location.href = `/Vendas/Create?clienteId=${clienteId}`;
    } else {
        window.location.href = '/Vendas/Create';
    }
};

window.novaAvaliacao = function (clienteId) {
    window.showLoading(true);
    if (clienteId) {
        window.location.href = `/Avaliacoes/Create?clienteId=${clienteId}`;
    } else {
        window.location.href = '/Avaliacoes/Create';
    }
};

window.venderVeiculo = function (idVeiculo) {
    window.showLoading(true);
    if (idVeiculo) {
        window.location.href = `/Vendas/Create?idVeiculo=${idVeiculo}`;
    }
};

window.editarRegistro = function (controller, id) {
    window.showLoading(true);
    window.location.href = `/${controller}/Edit/${id}`;
};

window.visualizarRegistro = function (controller, id) {
    window.showLoading(true);
    window.location.href = `/${controller}/Details/${id}`;
};

// ===================================================================
// HELPERS E UTILITÁRIOS
// ===================================================================

// Debounce helper
function debounce(func, wait, immediate) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            timeout = null;
            if (!immediate) func(...args);
        };
        const callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func(...args);
    };
}

// Throttle helper
function throttle(func, limit) {
    let inThrottle;
    return function () {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    }
}

// Formatação de moeda
window.formatCurrency = function (value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
};

// Formatação de número
window.formatNumber = function (value) {
    return new Intl.NumberFormat('pt-BR').format(value);
};

// Formatação de data
window.formatDate = function (value) {
    if (!value) return '-';
    const date = new Date(value);
    return date.toLocaleDateString('pt-BR');
};

// ===================================================================
// OBSERVADOR DE INTERSEÇÃO PARA LAZY LOADING
// ===================================================================
function setupIntersectionObserver() {
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '50px'
        });

        // Observar cards e elementos da grid
        document.querySelectorAll('.card-modern, .grid-row').forEach(el => {
            observer.observe(el);
        });
    }
}

function changePage(page) {
    const url = new URL(window.location);
    url.searchParams.set('page', page);
    window.location = url;
}

document.getElementById('pageSizeSelector')?.addEventListener('change', function () {
    const url = new URL(window.location);
    url.searchParams.set('pageSize', this.value);
    url.searchParams.set('page', '1');
    window.location = url;
});

// Sortable headers
document.querySelectorAll('.sortable-header').forEach(header => {
    header.addEventListener('click', function () {
        const sortColumn = this.dataset.sortable;
        const url = new URL(window.location);

        let newDirection = 'asc';
        if (url.searchParams.get('orderBy') === sortColumn && url.searchParams.get('orderDirection') === 'asc') {
            newDirection = 'desc';
        }

        url.searchParams.set('orderBy', sortColumn);
        url.searchParams.set('orderDirection', newDirection);
        url.searchParams.set('page', '1');

        window.location = url;
    });
});

// ===================================================================
// CORREÇÕES DE LAYOUT PARA GRID COMPACTA
// ===================================================================

document.addEventListener('DOMContentLoaded', function () {

    // ===================================================================
    // 1. CORREÇÃO DE Z-INDEX DINÂMICA
    // ===================================================================

    function fixGridZIndex() {
        // Encontrar o header principal da aplicação
        const headers = document.querySelectorAll('.navbar, .header, .topbar, .page-header, .app-header');
        const gridContainers = document.querySelectorAll('.data-grid, .table-responsive');
        const gridHeaders = document.querySelectorAll('.base-grid-table thead');

        // Definir z-index do header principal como máximo
        headers.forEach((header, index) => {
            header.style.zIndex = (1100 + index).toString();
            header.style.position = header.style.position || 'relative';
        });

        // Definir z-index da grid como baixo
        gridContainers.forEach(container => {
            container.style.zIndex = '1';
            container.style.position = 'relative';
        });

        // Definir z-index do header da tabela como médio
        gridHeaders.forEach(header => {
            header.style.zIndex = '5';
            header.style.position = 'sticky';
            header.style.top = '0';
        });
    }

    // ===================================================================
    // 2. CORREÇÃO DE DROPDOWNS
    // ===================================================================

    function fixDropdownZIndex() {
        const dropdowns = document.querySelectorAll('.dropdown-menu');
        dropdowns.forEach(dropdown => {
            dropdown.style.zIndex = '1050';
            dropdown.style.position = 'absolute';
        });

        // Observer para dropdowns que são criados dinamicamente
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === 1) { // Element node
                        const newDropdowns = node.querySelectorAll('.dropdown-menu');
                        newDropdowns.forEach(dropdown => {
                            dropdown.style.zIndex = '1050';
                            dropdown.style.position = 'absolute';
                        });
                    }
                });
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    // ===================================================================
    // 3. AJUSTE DINÂMICO DA ALTURA DA GRID
    // ===================================================================

    function adjustGridHeight() {
        const tableContainers = document.querySelectorAll('.table-responsive');

        tableContainers.forEach(container => {
            // Calcular altura disponível
            const windowHeight = window.innerHeight;
            const headerHeight = getHeaderHeight();
            const filtersHeight = getFiltersHeight();
            const controlsHeight = getControlsHeight();
            const paginationHeight = getPaginationHeight();
            const padding = 120; // Padding adicional para respiração

            const availableHeight = windowHeight - headerHeight - filtersHeight - controlsHeight - paginationHeight - padding;

            // Definir altura mínima e máxima
            const minHeight = 300;
            const maxHeight = Math.max(availableHeight, minHeight);

            container.style.maxHeight = maxHeight + 'px';
            container.style.minHeight = minHeight + 'px';
        });
    }

    function getHeaderHeight() {
        const pageHeader = document.querySelector('.page-header');
        const appHeader = document.querySelector('.navbar, .header, .topbar, .app-header');

        let height = 0;
        if (pageHeader) height += pageHeader.offsetHeight;
        if (appHeader) height += appHeader.offsetHeight;

        return height;
    }

    function getFiltersHeight() {
        const filters = document.querySelector('.search-filters');
        return filters ? filters.offsetHeight : 0;
    }

    function getControlsHeight() {
        const controls = document.querySelector('.grid-controls');
        return controls ? controls.offsetHeight : 0;
    }

    function getPaginationHeight() {
        const pagination = document.querySelector('.pagination-container');
        return pagination ? pagination.offsetHeight : 0;
    }

    // ===================================================================
    // 4. OTIMIZAÇÃO DE PERFORMANCE PARA SCROLL
    // ===================================================================

    function optimizeTableScroll() {
        const tableContainers = document.querySelectorAll('.table-responsive');

        tableContainers.forEach(container => {
            // Adicionar smooth scrolling
            container.style.scrollBehavior = 'smooth';

            // Throttle scroll events para melhor performance
            let scrollTimeout;
            container.addEventListener('scroll', function () {
                if (scrollTimeout) {
                    clearTimeout(scrollTimeout);
                }
                scrollTimeout = setTimeout(function () {
                    // Aqui você pode adicionar lógica adicional se necessário
                }, 100);
            });
        });
    }

    // ===================================================================
    // 5. CORREÇÃO DE LARGURAS DE COLUNAS EM TELAS PEQUENAS
    // ===================================================================

    function adjustColumnWidths() {
        const table = document.querySelector('.base-grid-table');
        if (!table) return;

        const screenWidth = window.innerWidth;

        if (screenWidth < 768) {
            // Mobile: ocultar colunas menos importantes
            const columnsToHide = [3, 4, 7]; // Índices das colunas (baseado em 1)

            columnsToHide.forEach(colIndex => {
                const headerCells = table.querySelectorAll(`th:nth-child(${colIndex})`);
                const bodyCells = table.querySelectorAll(`td:nth-child(${colIndex})`);

                [...headerCells, ...bodyCells].forEach(cell => {
                    cell.style.display = 'none';
                });
            });

        } else {
            // Desktop: mostrar todas as colunas
            const allCells = table.querySelectorAll('th, td');
            allCells.forEach(cell => {
                cell.style.display = '';
            });
        }
    }

    // ===================================================================
    // 6. DEBOUNCE PARA RESIZE
    // ===================================================================

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // ===================================================================
    // 7. INICIALIZAÇÃO E EVENT LISTENERS
    // ===================================================================

    // Executar correções iniciais
    fixGridZIndex();
    fixDropdownZIndex();
    adjustGridHeight();
    optimizeTableScroll();
    adjustColumnWidths();

    // Executar correções no resize da janela
    const debouncedResize = debounce(() => {
        adjustGridHeight();
        adjustColumnWidths();
    }, 250);

    window.addEventListener('resize', debouncedResize);

    // ===================================================================
    // 8. CORREÇÃO PARA CASOS ESPECÍFICOS
    // ===================================================================

    // Se houver problemas com elementos que aparecem dinamicamente
    setTimeout(() => {
        fixGridZIndex();
        adjustGridHeight();
    }, 500);

    // Observar mudanças no DOM para elementos dinâmicos
    const globalObserver = new MutationObserver(function (mutations) {
        let needsUpdate = false;

        mutations.forEach(function (mutation) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                mutation.addedNodes.forEach(function (node) {
                    if (node.nodeType === 1 &&
                        (node.classList.contains('data-grid') ||
                            node.classList.contains('table-responsive') ||
                            node.classList.contains('dropdown-menu'))) {
                        needsUpdate = true;
                    }
                });
            }
        });

        if (needsUpdate) {
            setTimeout(() => {
                fixGridZIndex();
                fixDropdownZIndex();
                adjustGridHeight();
            }, 100);
        }
    });

    globalObserver.observe(document.body, {
        childList: true,
        subtree: true
    });

    // ===================================================================
    // 9. FUNÇÃO PARA FORÇAR RECÁLCULO (caso necessário)
    // ===================================================================

    // Expor função global para recálculo manual se necessário
    window.recalculateGridLayout = function () {
        fixGridZIndex();
        fixDropdownZIndex();
        adjustGridHeight();
        optimizeTableScroll();
        adjustColumnWidths();
    };

    console.log('Grid layout fixes initialized successfully');
});

// ===================================================================
// 10. CSS ADICIONAL VIA JAVASCRIPT (caso necessário)
// ===================================================================

// Adicionar estilos críticos via JavaScript se não puderem ser aplicados via CSS
function addCriticalStyles() {
    const style = document.createElement('style');
    style.textContent = `
            /* Estilos críticos que devem ser aplicados imediatamente */
            .table-responsive {
                position: relative !important;
                z-index: 1 !important;
            }

            .base-grid-table thead {
                position: sticky !important;
                top: 0 !important;
                z-index: 5 !important;
            }

            .dropdown-menu {
                z-index: 1050 !important;
                position: absolute !important;
            }

            /* Prevenir flash de conteúdo não estilizado */
            .data-grid {
                opacity: 0;
                transition: opacity 0.3s ease;
            }

            .data-grid.loaded {
                opacity: 1;
            }
        `;

    document.head.appendChild(style);

    // Marcar grids como carregadas após um breve delay
    setTimeout(() => {
        document.querySelectorAll('.data-grid').forEach(grid => {
            grid.classList.add('loaded');
        });
    }, 100);
}

// Executar estilos críticos imediatamente
addCriticalStyles();

// Inicializar observer após carregamento
document.addEventListener('DOMContentLoaded', () => {
    setTimeout(setupIntersectionObserver, 500);
});

// ===================================================================
// EXPORTAÇÃO E IMPORTAÇÃO DE DADOS
// ===================================================================

window.exportarDados = function (controller, formato = 'csv') {
    const currentFilters = new URLSearchParams(window.location.search);
    const exportUrl = `/${controller}/Export?formato=${formato}&${currentFilters.toString()}`;

    window.showLoading(true);

    // Criar link temporário para download
    const link = document.createElement('a');
    link.href = exportUrl;
    link.download = '';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    setTimeout(() => {
        window.showLoading(false);
    }, 2000);
};

// ===================================================================
// SISTEMA DE NOTIFICAÇÕES MELHORADO
// ===================================================================

class ToastManager {
    constructor() {
        this.container = this.createContainer();
    }

    createContainer() {
        let container = document.querySelector('.toast-container-modern');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container-modern';
            container.innerHTML = `
                <style>
                .toast-container-modern {
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 10000;
                    max-width: 400px;
                }
                .toast-modern {
                    background: white;
                    border-radius: 8px;
                    box-shadow: 0 10px 25px rgba(0,0,0,0.1);
                    margin-bottom: 10px;
                    overflow: hidden;
                    transform: translateX(100%);
                    transition: transform 0.3s ease;
                }
                .toast-modern.show {
                    transform: translateX(0);
                }
                .toast-modern.hide {
                    transform: translateX(100%);
                }
                .toast-header-modern {
                    display: flex;
                    align-items: center;
                    padding: 12px 16px;
                    border-bottom: 1px solid #f0f0f0;
                }
                .toast-icon {
                    width: 24px;
                    height: 24px;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin-right: 8px;
                    font-size: 12px;
                }
                .toast-icon.success { background: #d1fae5; color: #065f46; }
                .toast-icon.error { background: #fee2e2; color: #991b1b; }
                .toast-icon.warning { background: #fef3c7; color: #92400e; }
                .toast-icon.info { background: #dbeafe; color: #1e40af; }
                .toast-title {
                    font-weight: 600;
                    flex: 1;
                }
                .toast-close {
                    background: none;
                    border: none;
                    color: #6b7280;
                    cursor: pointer;
                    padding: 4px;
                }
                .toast-body-modern {
                    padding: 12px 16px;
                    color: #374151;
                }
                .toast-progress {
                    height: 3px;
                    background: #f3f4f6;
                    overflow: hidden;
                }
                .toast-progress-bar {
                    height: 100%;
                    background: currentColor;
                    animation: progress 5s linear;
                }
                @keyframes progress {
                    from { width: 100%; }
                    to { width: 0%; }
                }
                </style>
            `;
            document.body.appendChild(container);
        }
        return container;
    }

    show(message, type = 'info', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = 'toast-modern';

        const iconMap = {
            success: 'fas fa-check',
            error: 'fas fa-times',
            warning: 'fas fa-exclamation',
            info: 'fas fa-info'
        };

        const titleMap = {
            success: 'Sucesso',
            error: 'Erro',
            warning: 'Atenção',
            info: 'Informação'
        };

        toast.innerHTML = `
            <div class="toast-header-modern">
                <div class="toast-icon ${type}">
                    <i class="${iconMap[type]}"></i>
                </div>
                <div class="toast-title">${titleMap[type]}</div>
                <button class="toast-close" onclick="this.closest('.toast-modern').classList.add('hide')">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="toast-body-modern">${message}</div>
            <div class="toast-progress">
                <div class="toast-progress-bar"></div>
            </div>
        `;

        this.container.appendChild(toast);

        setTimeout(() => toast.classList.add('show'), 100);
        setTimeout(() => this.hide(toast), duration);
    }

    hide(toast) {
        toast.classList.add('hide');
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }
}

// Instanciar gerenciador de toast
const toastManager = new ToastManager();

// Sobrescrever função global de toast
window.showToast = function (message, type = 'info', duration = 5000) {
    toastManager.show(message, type, duration);
};

// ===================================================================
// INICIALIZAÇÃO FINAL E EXPORTAÇÕES
// ===================================================================

// Aguardar que tudo esteja carregado
window.addEventListener('load', function () {
    console.log('📋 Sistema de Grid completamente carregado');

    // Esconder qualquer loading residual
    setTimeout(() => {
        if (window.forceHideLoading) {
            window.forceHideLoading();
        }
    }, 1000);
});

// Exportar instância para debug
window.StandardGrid = StandardGrid;
window.gridInstance = gridInstance;

// ===================================================================
// DROPDOWN PORTAL SYSTEM - SOLUÇÃO DEFINITIVA
// ===================================================================

class DropdownPortalSystem {
    constructor() {
        this.activeDropdown = null;
        this.dropdownElement = null;
        this.backdropElement = null;
        this.portalContainer = null;
        this.isInitialized = false;

        this.init();
    }

    init() {
        if (this.isInitialized) return;

        this.setupPortalContainer();
        this.setupEventListeners();
        this.isInitialized = true;

        console.log('✅ Dropdown Portal System initialized');
    }

    setupPortalContainer() {
        // Criar container do portal se não existir
        this.portalContainer = document.getElementById('dropdownPortal');
        if (!this.portalContainer) {
            this.portalContainer = document.createElement('div');
            this.portalContainer.id = 'dropdownPortal';
            this.portalContainer.className = 'dropdown-portal';
            document.body.appendChild(this.portalContainer);
        }
    }

    setupEventListeners() {
        // Event delegation para botões de ação
        document.addEventListener('click', (e) => {
            const actionBtn = e.target.closest('.btn-actions-enhanced');

            if (actionBtn) {
                e.preventDefault();
                e.stopPropagation();
                this.handleDropdownToggle(actionBtn);
            } else if (!e.target.closest('.dropdown-menu-portal')) {
                this.closeDropdown();
            }
        });

        // Fechar no ESC
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.activeDropdown) {
                this.closeDropdown();
            }
        });

        // Fechar no scroll
        window.addEventListener('scroll', () => {
            if (this.activeDropdown) {
                this.closeDropdown();
            }
        }, true);

        // Fechar no resize
        window.addEventListener('resize', () => {
            if (this.activeDropdown) {
                this.closeDropdown();
            }
        });
    }

    handleDropdownToggle(button) {
        const container = button.closest('.action-dropdown-container');
        const itemId = container?.dataset.itemId;

        if (!container || !itemId) {
            console.error('Container ou itemId não encontrado');
            return;
        }

        // Se é o mesmo dropdown, fechar
        if (this.activeDropdown === itemId) {
            this.closeDropdown();
            return;
        }

        // Fechar dropdown anterior se existir
        if (this.activeDropdown) {
            this.closeDropdown();
        }

        // Abrir novo dropdown
        this.openDropdown(button, container, itemId);
    }

    openDropdown(button, container, itemId) {
        try {
            // Calcular posição
            const position = this.calculateDropdownPosition(button);

            // Obter dados das ações
            const actionsData = this.getActionsData(container);
            if (!actionsData || !actionsData.actions) {
                console.error('Dados de ações não encontrados');
                return;
            }

            // Criar dropdown
            this.createDropdownElement(actionsData.actions, position);

            // Criar backdrop
            this.createBackdrop();

            // Marcar como ativo
            this.activeDropdown = itemId;
            button.setAttribute('aria-expanded', 'true');

            // Adicionar classes CSS
            const tableContainer = document.querySelector('.table-responsive-enhanced');
            const actionsCell = button.closest('.actions-cell');

            if (tableContainer) tableContainer.classList.add('dropdown-active');
            if (actionsCell) actionsCell.classList.add('dropdown-active');

            console.log(`✅ Dropdown aberto para item ${itemId}`);

        } catch (error) {
            console.error('Erro ao abrir dropdown:', error);
        }
    }

    calculateDropdownPosition(button) {
        const rect = button.getBoundingClientRect();
        const viewportHeight = window.innerHeight;
        const viewportWidth = window.innerWidth;
        const dropdownHeight = 250; // Altura estimada
        const dropdownWidth = 180;

        let top = rect.bottom + window.scrollY + 5;
        let left = rect.right + window.scrollX - dropdownWidth;
        let direction = 'down';

        // Verificar se cabe embaixo
        if (rect.bottom + dropdownHeight > viewportHeight) {
            top = rect.top + window.scrollY - dropdownHeight - 5;
            direction = 'up';
        }

        // Verificar se cabe à direita
        if (left < 10) {
            left = rect.left + window.scrollX;
        }

        // Garantir que não saia da tela
        if (left + dropdownWidth > viewportWidth - 10) {
            left = viewportWidth - dropdownWidth - 10;
        }

        return { top, left, direction };
    }

    getActionsData(container) {
        try {
            const dataScript = container.querySelector('.dropdown-data');
            if (!dataScript) {
                console.error('Script de dados não encontrado');
                return null;
            }

            return JSON.parse(dataScript.textContent);
        } catch (error) {
            console.error('Erro ao parsear dados do dropdown:', error);
            return null;
        }
    }

    createDropdownElement(actions, position) {
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

                // Event listener para ações
                item.addEventListener('click', (e) => {
                    if (action.name.toLowerCase().includes('delete') || action.name.toLowerCase().includes('excluir')) {
                        e.preventDefault();
                        if (confirm('Tem certeza que deseja excluir este registro?')) {
                            window.location.href = action.url;
                        }
                    } else if (action.url && action.url !== '#') {
                        // Deixa o comportamento padrão do link
                        this.closeDropdown();
                    } else {
                        e.preventDefault();
                        console.log(`Ação executada: ${action.name}`);
                    }
                });

                this.dropdownElement.appendChild(item);
            }
        });

        // Adicionar ao portal
        this.portalContainer.appendChild(this.dropdownElement);

        // Mostrar com animação
        setTimeout(() => {
            this.dropdownElement.classList.add('show');
            this.dropdownElement.style.setProperty('position', 'fixed', 'important');
            this.dropdownElement.style.setProperty('top', `${position.top - 50}px`, 'important');
            this.dropdownElement.style.setProperty('left', `${position.left}px`, 'important');
        }, 3);
    }

    createBackdrop() {
        if (this.backdropElement) {
            this.backdropElement.remove();
        }

        this.backdropElement = document.createElement('div');
        this.backdropElement.className = 'dropdown-backdrop';
        this.backdropElement.addEventListener('click', () => {
            this.closeDropdown();
        });

        document.body.appendChild(this.backdropElement);
    }

    closeDropdown() {
        if (!this.activeDropdown) return;

        try {
            // Remover elementos
            if (this.dropdownElement) {
                this.dropdownElement.classList.remove('show');
                setTimeout(() => {
                    if (this.dropdownElement) {
                        this.dropdownElement.remove();
                        this.dropdownElement = null;
                    }
                }, 50);
            }

            if (this.backdropElement) {
                this.backdropElement.remove();
                this.backdropElement = null;
            }

            // Remover classes CSS
            const tableContainer = document.querySelector('.table-responsive-enhanced');
            const actionsCells = document.querySelectorAll('.actions-cell.dropdown-active');
            const buttons = document.querySelectorAll('.btn-actions-enhanced[aria-expanded="true"]');

            if (tableContainer) tableContainer.classList.remove('dropdown-active');
            actionsCells.forEach(cell => cell.classList.remove('dropdown-active'));
            buttons.forEach(btn => btn.setAttribute('aria-expanded', 'false'));

            this.activeDropdown = null;

            console.log('✅ Dropdown fechado');

        } catch (error) {
            console.error('Erro ao fechar dropdown:', error);
        }
    }

    // Método público para forçar fechamento
    forceClose() {
        this.closeDropdown();
    }

    // Método para reinicializar após updates da grid
    reinitialize() {
        this.closeDropdown();
        this.setupPortalContainer();
    }
}

// ===================================================================
// INICIALIZAÇÃO AUTOMÁTICA
// ===================================================================

document.addEventListener('DOMContentLoaded', () => {
    // Inicializar sistema de dropdown
    window.dropdownPortalSystem = new DropdownPortalSystem();

    // Integrar com sistema de grid existente
    if (typeof gridInstance !== 'undefined') {
        gridInstance.dropdownSystem = window.dropdownPortalSystem;

        // Reinicializar após updates da grid
        document.addEventListener('gridUpdated', () => {
            window.dropdownPortalSystem.reinitialize();
        });
    }

    // Expor métodos globais
    window.closeAllDropdowns = () => {
        if (window.dropdownPortalSystem) {
            window.dropdownPortalSystem.forceClose();
        }
    };
});

// ===================================================================
// INTEGRAÇÃO COM SISTEMA EXISTENTE
// ===================================================================

// Override da função de confirmação de exclusão se existir
if (typeof confirmarExclusao === 'function') {
    window.confirmarExclusaoOriginal = window.confirmarExclusao;
}

window.confirmarExclusao = function (id) {
    // Usar modal de confirmação padrão do sistema, depois modal de resultado
    if (confirm('Tem certeza que deseja excluir este registro? Esta ação não pode ser desfeita.')) {
        const currentPath = window.location.pathname.toLowerCase();
        let controller = '';

        if (currentPath.includes('veiculos')) {
            controller = 'Veiculos';
        } else if (currentPath.includes('clientes')) {
            controller = 'Clientes';
        } else if (currentPath.includes('vendedores')) {
            controller = 'Vendedores';
        }

        if (controller) {
            window.showLoading(true);

            // Simular exclusão e mostrar resultado
            fetch(`/${controller}/Delete/${id}`, {
                method: 'POST'
            })
                .then(response => response.json())
                .then(result => {
                    window.showLoading(false);

                    if (result.sucesso) {
                        showSuccess('Registro excluído com sucesso!', {
                            title: 'Exclusão Realizada!',
                            buttonText: 'OK'
                        }).then(() => {
                            // Recarregar grid após fechar modal
                            location.reload();
                        });
                    } else {
                        showError('Não foi possível excluir o registro. ' + (result.mensagem || ''), {
                            title: 'Erro na Exclusão',
                            buttonText: 'Entendi'
                        });
                    }
                })
                .catch(error => {
                    window.showLoading(false);
                    showError('Erro de conexão durante a exclusão.', {
                        title: 'Erro de Sistema',
                        buttonText: 'Tentar Novamente'
                    });
                });
        }
    }
};