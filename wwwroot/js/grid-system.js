/*!
 * Auto-Gestão Grid System v2.0
 * Sistema unificado de grid para todas as listagens
 * Copyright 2025 Auto-Gestão
 */

(function () {
    'use strict';

    // ===================================================================
    // VARIÁVEIS GLOBAIS
    // ===================================================================
    let isLoadingVisible = false;
    let loadingTimeout = null;
    let searchTimeout = null;
    let isProcessingRequest = false;

    // ===================================================================
    // SISTEMA DE LOADING
    // ===================================================================
    function showLoading(show) {
        const overlay = document.getElementById('loadingOverlay');

        if (!overlay) {
            console.warn('Loading overlay não encontrado');
            return;
        }

        // Limpar timeout anterior
        if (loadingTimeout) {
            clearTimeout(loadingTimeout);
            loadingTimeout = null;
        }

        if (show && !isLoadingVisible) {
            isLoadingVisible = true;
            isProcessingRequest = true;

            overlay.classList.remove('d-none');
            overlay.style.display = 'flex';
            overlay.style.opacity = '1';
            overlay.style.visibility = 'visible';

            // Adicionar classe loading à tabela
            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.add('loading');
            }

            // Timeout de segurança - forçar esconder após 15 segundos
            loadingTimeout = setTimeout(() => {
                console.warn('Loading forçado a esconder após timeout');
                showLoading(false);
            }, 15000);

        } else if (!show && isLoadingVisible) {
            isLoadingVisible = false;
            isProcessingRequest = false;

            overlay.style.opacity = '0';

            // Remover classe loading da tabela
            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.remove('loading');
            }

            // Aguardar transição antes de esconder completamente
            setTimeout(() => {
                overlay.classList.add('d-none');
                overlay.style.display = 'none';
                overlay.style.visibility = 'hidden';
            }, 300);
        }
    }

    function forceHideLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            isLoadingVisible = false;
            isProcessingRequest = false;

            overlay.classList.add('d-none');
            overlay.style.display = 'none';
            overlay.style.opacity = '0';
            overlay.style.visibility = 'hidden';

            const table = document.querySelector('.base-grid-table');
            if (table) {
                table.classList.remove('loading');
            }

            if (loadingTimeout) {
                clearTimeout(loadingTimeout);
                loadingTimeout = null;
            }
        }
    }

    // ===================================================================
    // SISTEMA DE FILTROS E AJAX
    // ===================================================================
    function aplicarFiltros(page = 1) {
        // Prevenir múltiplas requisições
        if (isProcessingRequest) {
            console.log('Requisição já em andamento, ignorando...');
            return;
        }

        showLoading(true);

        const form = document.getElementById('filtrosForm');
        if (!form) {
            console.error('Formulário de filtros não encontrado');
            forceHideLoading();
            return;
        }

        const formData = new FormData(form);
        const pageSize = document.getElementById('pageSizeSelector')?.value || 50;

        const params = new URLSearchParams();
        params.append('page', page);
        params.append('pageSize', pageSize);

        // Adicionar filtros apenas se tiverem valor
        for (let [key, value] of formData.entries()) {
            if (value && value.trim() !== '') {
                params.append(key, value);
            }
        }

        // Determinar URL AJAX baseado na URL atual
        const currentPath = window.location.pathname.toLowerCase();
        let ajaxUrl = '';

        if (currentPath.includes('/veiculos')) {
            ajaxUrl = '/Veiculos/GetVeiculosAjax';
        } else if (currentPath.includes('/clientes')) {
            ajaxUrl = '/Clientes/GetClientesAjax';
        } else {
            console.error('Controlador não identificado para:', currentPath);
            forceHideLoading();
            return;
        }

        const fullUrl = `${ajaxUrl}?${params.toString()}`;
        console.log(`🔄 Fazendo requisição: ${fullUrl}`);

        // Fazer requisição AJAX
        fetch(fullUrl)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Erro HTTP: ${response.status} - ${response.statusText}`);
                }
                return response.text();
            })
            .then(html => {
                const gridContainer = document.getElementById('gridContainer');
                if (gridContainer) {
                    // Atualizar conteúdo
                    gridContainer.innerHTML = html;

                    // Atualizar URL sem recarregar página
                    updateUrl(params);

                    // Reinicializar eventos
                    initializeGridEvents();

                    // Disparar evento customizado
                    document.dispatchEvent(new CustomEvent('gridUpdated', {
                        detail: { page, params: params.toString() }
                    }));

                    console.log('✅ Grid atualizada com sucesso');
                } else {
                    throw new Error('Container da grid não encontrado');
                }
            })
            .catch(error => {
                console.error('❌ Erro ao carregar dados:', error);

                // Mostrar mensagem de erro para o usuário
                if (window.showToast) {
                    showToast(`Erro ao carregar dados: ${error.message}`, 'error');
                } else {
                    alert(`Erro ao carregar dados: ${error.message}`);
                }

                // Marcar linhas com erro se a grid ainda estiver visível
                const rows = document.querySelectorAll('.grid-row');
                rows.forEach(row => row.classList.add('grid-row-error'));

                setTimeout(() => {
                    rows.forEach(row => row.classList.remove('grid-row-error'));
                }, 2000);
            })
            .finally(() => {
                // Sempre esconder loading após um delay mínimo para UX
                setTimeout(() => {
                    showLoading(false);
                }, 300);
            });
    }

    function updateUrl(params) {
        try {
            const newUrl = `${window.location.pathname}?${params.toString()}`;
            window.history.replaceState({ gridParams: params.toString() }, '', newUrl);
        } catch (error) {
            console.error('Erro ao atualizar URL:', error);
        }
    }

    // ===================================================================
    // EVENTOS DA GRID
    // ===================================================================
    function initializeGridEvents() {
        // Duplo clique nas linhas para abrir detalhes
        const rows = document.querySelectorAll('.grid-row');
        rows.forEach(row => {
            // Remover listener anterior para evitar duplicação
            row.removeEventListener('dblclick', handleRowDoubleClick);
            row.addEventListener('dblclick', handleRowDoubleClick);

            // Adicionar classe para cursor pointer
            row.style.cursor = 'pointer';

            // Adicionar tooltip
            const controller = row.getAttribute('data-controller');
            row.title = `Duplo-clique para ver detalhes${controller ? ` do ${controller.toLowerCase()}` : ''}`;
        });

        // Ordenação das colunas
        const sortIcons = document.querySelectorAll('.sort-icon');
        sortIcons.forEach(icon => {
            icon.removeEventListener('click', handleSortClick);
            icon.addEventListener('click', handleSortClick);

            // Melhorar acessibilidade
            const header = icon.closest('th');
            if (header) {
                header.style.cursor = 'pointer';
                header.setAttribute('tabindex', '0');
                header.setAttribute('role', 'columnheader');
                header.setAttribute('aria-sort', 'none');
            }
        });

        // Atualizar estado de ordenação visual
        updateSortVisualState();
    }

    function handleRowDoubleClick(e) {
        // Evitar conflito com elementos interativos
        if (e.target.closest('.dropdown') ||
            e.target.closest('.actions-btn') ||
            e.target.closest('button') ||
            e.target.closest('a')) {
            return;
        }

        const id = this.getAttribute('data-id');
        const controller = this.getAttribute('data-controller');

        if (id && controller) {
            // Adicionar efeito visual
            this.classList.add('grid-row-success');

            showLoading(true);

            // Navegar para detalhes
            setTimeout(() => {
                window.location.href = `/${controller}/Details/${id}`;
            }, 200);
        } else {
            console.warn('ID ou Controller não encontrado na linha:', this);
        }
    }

    function handleSortClick(e) {
        e.stopPropagation();
        e.preventDefault();

        const sortKey = this.getAttribute('data-sort');
        if (sortKey) {
            sortColumn(sortKey);
        }
    }

    function updateSortVisualState() {
        const currentOrder = getCurrentSortOrder();
        const currentDirection = getCurrentSortDirection();

        // Resetar todos os ícones
        document.querySelectorAll('.sort-icon').forEach(icon => {
            icon.classList.remove('fa-sort-up', 'fa-sort-down');
            icon.classList.add('fa-sort');

            const header = icon.closest('th');
            if (header) {
                header.setAttribute('aria-sort', 'none');
            }
        });

        // Atualizar ícone ativo
        const activeIcon = document.querySelector(`[data-sort="${currentOrder}"]`);
        if (activeIcon) {
            activeIcon.classList.remove('fa-sort');
            if (currentDirection === 'asc') {
                activeIcon.classList.add('fa-sort-up');
                activeIcon.closest('th')?.setAttribute('aria-sort', 'ascending');
            } else {
                activeIcon.classList.add('fa-sort-down');
                activeIcon.closest('th')?.setAttribute('aria-sort', 'descending');
            }
        }
    }

    // ===================================================================
    // FUNÇÕES DE ORDENAÇÃO
    // ===================================================================
    function sortColumn(column) {
        // Buscar ou criar campos hidden de ordenação
        let orderBy = document.getElementById('orderBy');
        let orderDirection = document.getElementById('orderDirection');

        const form = document.getElementById('filtrosForm');
        if (!form) {
            console.error('Formulário não encontrado');
            return;
        }

        // Criar campos se não existirem
        if (!orderBy) {
            orderBy = document.createElement('input');
            orderBy.type = 'hidden';
            orderBy.id = 'orderBy';
            orderBy.name = 'orderBy';
            orderBy.value = 'Nome';
            form.appendChild(orderBy);
        }

        if (!orderDirection) {
            orderDirection = document.createElement('input');
            orderDirection.type = 'hidden';
            orderDirection.id = 'orderDirection';
            orderDirection.name = 'orderDirection';
            orderDirection.value = 'asc';
            form.appendChild(orderDirection);
        }

        const currentOrder = orderBy.value;
        const currentDirection = orderDirection.value;

        // Lógica de ordenação
        if (currentOrder === column) {
            // Se já está ordenando por esta coluna, inverte direção
            orderDirection.value = currentDirection === 'asc' ? 'desc' : 'asc';
        } else {
            // Nova coluna, sempre começar crescente
            orderBy.value = column;
            orderDirection.value = 'asc';
        }

        console.log(`🔄 Ordenando por: ${orderBy.value} (${orderDirection.value})`);

        // Aplicar filtros com nova ordenação
        aplicarFiltros(1); // Voltar para primeira página
    }

    function getCurrentSortOrder() {
        const orderBy = document.getElementById('orderBy');
        return orderBy ? orderBy.value : 'Nome';
    }

    function getCurrentSortDirection() {
        const orderDirection = document.getElementById('orderDirection');
        return orderDirection ? orderDirection.value : 'asc';
    }

    // ===================================================================
    // INICIALIZAÇÃO DOS FILTROS
    // ===================================================================
    function initializeFilters() {
        // Filtro de busca com debounce
        const searchInputs = [
            'searchInput',
            'searchClientes',
            'searchVeiculos',
            document.querySelector('input[name="search"]')
        ].filter(Boolean);

        searchInputs.forEach(input => {
            if (typeof input === 'string') {
                input = document.getElementById(input);
            }

            if (input) {
                input.addEventListener('input', function () {
                    clearTimeout(searchTimeout);

                    // Mostrar indicador visual de que está digitando
                    this.style.borderColor = '#fbbf24';

                    searchTimeout = setTimeout(() => {
                        this.style.borderColor = '';
                        aplicarFiltros(1);
                    }, 600); // 600ms de debounce
                });

                // Limpar ao pressionar Escape
                input.addEventListener('keydown', function (e) {
                    if (e.key === 'Escape') {
                        this.value = '';
                        aplicarFiltros(1);
                    }
                });
            }
        });

        // Filtros que aplicam imediatamente
        const immediateFilters = [
            'filterSituacao', 'filterTipo', 'filterStatus',
            'filterMarca', 'filterAno', 'filterCombustivel'
        ];

        immediateFilters.forEach(filterId => {
            const element = document.getElementById(filterId);
            if (element) {
                element.addEventListener('change', () => {
                    console.log(`🔧 Filtro alterado: ${filterId} = ${element.value}`);
                    aplicarFiltros(1);
                });
            }
        });

        // Seletor de quantidade por página
        const pageSizeSelector = document.getElementById('pageSizeSelector');
        if (pageSizeSelector) {
            pageSizeSelector.addEventListener('change', function () {
                console.log(`📄 Tamanho da página alterado: ${this.value}`);
                aplicarFiltros(1);
            });
        }
    }

    // ===================================================================
    // FUNÇÕES UTILITÁRIAS
    // ===================================================================
    function changePage(page) {
        console.log(`📄 Mudando para página: ${page}`);
        aplicarFiltros(page);
    }

    function limparFiltros() {
        console.log('🧹 Limpando todos os filtros');

        const form = document.getElementById('filtrosForm');
        if (form) {
            form.reset();
        }

        const pageSizeSelector = document.getElementById('pageSizeSelector');
        if (pageSizeSelector) {
            pageSizeSelector.value = '50';
        }

        // Limpar campos hidden de ordenação
        const orderBy = document.getElementById('orderBy');
        const orderDirection = document.getElementById('orderDirection');

        if (orderBy) orderBy.value = 'Nome';
        if (orderDirection) orderDirection.value = 'asc';

        aplicarFiltros(1);
    }

    function confirmarExclusao(id) {
        if (!id) {
            console.error('ID não fornecido para exclusão');
            return;
        }

        if (confirm('Tem certeza que deseja excluir este registro?\n\nEsta ação não pode ser desfeita.')) {
            const currentPath = window.location.pathname.toLowerCase();
            let controller = '';

            if (currentPath.includes('/veiculos')) {
                controller = 'Veiculos';
            } else if (currentPath.includes('/clientes')) {
                controller = 'Clientes';
            }

            if (controller) {
                showLoading(true);
                window.location.href = `/${controller}/Delete/${id}`;
            } else {
                console.error('Controlador não identificado para exclusão');
            }
        }
    }

    function exportarExcel() {
        if (window.showToast) {
            showToast('Preparando exportação...', 'info', 2000);

            // Simular exportação
            setTimeout(() => {
                showToast('Exportação concluída!', 'success');
            }, 2000);
        } else {
            alert('Exportação iniciada!');
        }
    }

    // ===================================================================
    // FUNÇÕES ESPECÍFICAS POR CONTROLADOR
    // ===================================================================
    function novaVenda(clienteId) {
        const url = clienteId ? `/Vendas/Create?clienteId=${clienteId}` : '/Vendas/Create';
        window.location.href = url;
    }

    function novaAvaliacao(clienteId) {
        const url = clienteId ? `/Avaliacoes/Create?clienteId=${clienteId}` : '/Avaliacoes/Create';
        window.location.href = url;
    }

    function venderVeiculo(veiculoId) {
        if (veiculoId) {
            window.location.href = `/Vendas/Create?veiculoId=${veiculoId}`;
        }
    }

    // ===================================================================
    // SISTEMA DE PROTEÇÃO E RECOVERY
    // ===================================================================
    function setupProtectionSystems() {
        // Proteção contra loading infinito
        setTimeout(forceHideLoading, 2000);

        // Listener para fim do carregamento da página
        window.addEventListener('load', () => {
            setTimeout(forceHideLoading, 1500);
        });

        // Proteção contra erros JavaScript
        window.addEventListener('error', (event) => {
            console.error('Erro JavaScript detectado:', event.error);
            forceHideLoading();
        });

        // Proteção para mudanças de visibilidade
        document.addEventListener('visibilitychange', () => {
            if (document.visibilityState === 'visible' && isLoadingVisible) {
                setTimeout(() => {
                    if (isLoadingVisible) {
                        console.warn('Loading ainda visível após mudança de visibilidade');
                        forceHideLoading();
                    }
                }, 3000);
            }
        });

        // Proteção para navegação do browser
        window.addEventListener('beforeunload', () => {
            forceHideLoading();
        });

        // Proteção por timeout global
        setInterval(() => {
            if (isLoadingVisible && !isProcessingRequest) {
                console.warn('Loading órfão detectado, forçando limpeza');
                forceHideLoading();
            }
        }, 30000); // Verificar a cada 30 segundos
    }

    // ===================================================================
    // MELHORIAS DE ACESSIBILIDADE
    // ===================================================================
    function setupAccessibility() {
        // Navegação por teclado na grid
        document.addEventListener('keydown', (e) => {
            const activeElement = document.activeElement;

            if (activeElement && activeElement.classList.contains('grid-row')) {
                switch (e.key) {
                    case 'Enter':
                        e.preventDefault();
                        activeElement.dispatchEvent(new MouseEvent('dblclick'));
                        break;

                    case 'ArrowDown':
                        e.preventDefault();
                        const nextRow = activeElement.nextElementSibling;
                        if (nextRow && nextRow.classList.contains('grid-row')) {
                            nextRow.focus();
                        }
                        break;

                    case 'ArrowUp':
                        e.preventDefault();
                        const prevRow = activeElement.previousElementSibling;
                        if (prevRow && prevRow.classList.contains('grid-row')) {
                            prevRow.focus();
                        }
                        break;
                }
            }

            // Atalhos globais
            if (e.ctrlKey || e.metaKey) {
                switch (e.key) {
                    case 'f':
                        e.preventDefault();
                        const searchInput = document.querySelector('input[name="search"]') ||
                            document.getElementById('searchInput') ||
                            document.getElementById('searchClientes');
                        if (searchInput) {
                            searchInput.focus();
                            searchInput.select();
                        }
                        break;

                    case 'r':
                        e.preventDefault();
                        limparFiltros();
                        break;
                }
            }

            // Escape para limpar filtros
            if (e.key === 'Escape') {
                const activeInput = document.activeElement;
                if (activeInput && activeInput.tagName === 'INPUT') {
                    activeInput.blur();
                }
            }
        });

        // Adicionar ARIA labels
        const table = document.querySelector('.base-grid-table');
        if (table) {
            table.setAttribute('role', 'grid');
            table.setAttribute('aria-label', 'Tabela de dados com filtros e ordenação');
        }

        // Tornar linhas focáveis
        document.querySelectorAll('.grid-row').forEach((row, index) => {
            row.setAttribute('tabindex', '0');
            row.setAttribute('role', 'row');
            row.setAttribute('aria-rowindex', index + 2);
        });
    }

    // ===================================================================
    // SISTEMA DE DEBUG E MONITORAMENTO
    // ===================================================================
    function setupDebugging() {
        // Debug da grid
        window.debugGrid = function () {
            const overlay = document.getElementById('loadingOverlay');
            const table = document.querySelector('.base-grid-table');
            const rows = document.querySelectorAll('.grid-row');

            console.group('🔍 GRID DEBUG');
            console.log('Loading State:', {
                visible: isLoadingVisible,
                processing: isProcessingRequest,
                overlay: !!overlay,
                overlayClasses: overlay?.className
            });
            console.log('Grid State:', {
                table: !!table,
                rows: rows.length,
                hasForm: !!document.getElementById('filtrosForm')
            });
            console.log('Current Filters:', getCurrentFilters());
            console.groupEnd();
        };

        // Monitoramento de performance
        if (window.performance && performance.mark) {
            window.markGridStart = () => performance.mark('grid-start');
            window.markGridEnd = () => {
                performance.mark('grid-end');
                performance.measure('grid-update', 'grid-start', 'grid-end');
                const measure = performance.getEntriesByName('grid-update')[0];
                console.log(`⚡ Grid atualizada em ${measure.duration.toFixed(2)}ms`);
            };
        }

        // Logs estruturados
        const originalLog = console.log;
        window.gridLog = function (message, data = null) {
            const timestamp = new Date().toISOString();
            originalLog(`🔧 [${timestamp}] GRID: ${message}`, data || '');
        };
    }

    function getCurrentFilters() {
        const form = document.getElementById('filtrosForm');
        if (!form) return {};

        const formData = new FormData(form);
        const filters = {};

        for (let [key, value] of formData.entries()) {
            if (value && value.trim() !== '') {
                filters[key] = value;
            }
        }

        return filters;
    }

    // ===================================================================
    // INICIALIZAÇÃO PRINCIPAL
    // ===================================================================
    function initializeGridSystem() {
        console.log('🚀 Inicializando Auto-Gestão Grid System v2.0...');

        try {
            // Configurar sistemas de proteção
            setupProtectionSystems();

            // Inicializar filtros
            initializeFilters();

            // Inicializar eventos da grid
            initializeGridEvents();

            // Configurar acessibilidade
            setupAccessibility();

            // Configurar debugging
            setupDebugging();

            // Forçar limpeza inicial
            forceHideLoading();

            // Escutar eventos customizados
            document.addEventListener('gridUpdated', (e) => {
                console.log('📡 Grid atualizada:', e.detail);
            });

            console.log('✅ Grid System inicializado com sucesso');

        } catch (error) {
            console.error('❌ Erro na inicialização do Grid System:', error);
            forceHideLoading();
        }
    }

    // ===================================================================
    // AUTO-INICIALIZAÇÃO
    // ===================================================================
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeGridSystem);
    } else {
        initializeGridSystem();
    }

    // ===================================================================
    // EXPOSIÇÃO DE FUNÇÕES GLOBAIS
    // ===================================================================

    // Funções principais
    window.aplicarFiltros = aplicarFiltros;
    window.changePage = changePage;
    window.limparFiltros = limparFiltros;
    window.sortColumn = sortColumn;
    window.confirmarExclusao = confirmarExclusao;
    window.exportarExcel = exportarExcel;

    // Funções específicas
    window.novaVenda = novaVenda;
    window.novaAvaliacao = novaAvaliacao;
    window.venderVeiculo = venderVeiculo;

    // Funções de sistema
    window.showLoading = showLoading;
    window.forceHideLoading = forceHideLoading;
    window.initializeGridEvents = initializeGridEvents;

    // Namespace para desenvolvimento
    window.GridSystem = {
        version: '2.0',
        aplicarFiltros,
        changePage,
        limparFiltros,
        sortColumn,
        showLoading,
        forceHideLoading,
        initializeGridEvents,
        getCurrentFilters,
        isLoading: () => isLoadingVisible,
        isProcessing: () => isProcessingRequest
    };

    console.log('📦 Auto-Gestão Grid System carregado e pronto!');

})();

// ===================================================================
// HELPER FUNCTIONS GLOBAIS
// ===================================================================

// Debounce utility
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

// Throttle utility
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

// Utility para detectar se é mobile
function isMobile() {
    return window.innerWidth <= 768;
}

// Utility para detectar se prefere movimento reduzido
function prefersReducedMotion() {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
}

// Event listener melhorado
function addEventListenerOnce(element, event, handler) {
    element.removeEventListener(event, handler);
    element.addEventListener(event, handler);
}

console.log('🛠️ Grid System utilities carregados');