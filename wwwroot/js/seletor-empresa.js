// wwwroot/js/seletor-empresa.js
class SeletorEmpresa {
    constructor() {
        this.empresas = [];
        this.empresaAtualId = null;
        this.isAdmin = false;
    }

    async init() {
        console.log('SeletorEmpresa inicializado');
        await this.carregarEmpresas();
        this.setupEventListeners();
    }

    async carregarEmpresas() {
        const container = document.getElementById('seletor-empresa-container');

        // Se não existe o container, significa que é admin ou não está autenticado
        if (!container) {
            console.log('SeletorEmpresa: Container não encontrado (provavelmente é admin)');
            return;
        }

        try {
            console.log('SeletorEmpresa: Buscando empresas vinculadas...');

            const response = await fetch('/Login/ObterEmpresasVinculadas', {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao carregar empresas: ${response.status}`);
            }

            const result = await response.json();
            console.log('SeletorEmpresa: Resposta recebida:', result);

            if (!result.success) {
                console.error('Erro ao carregar empresas:', result.message);
                container.style.display = 'none';
                return;
            }

            // Se for admin, esconder (redundante, mas por garantia)
            if (result.isAdmin) {
                console.log('SeletorEmpresa: Usuário é admin, ocultando seletor');
                this.isAdmin = true;
                container.style.display = 'none';
                return;
            }

            this.empresas = result.empresas || [];
            this.empresaAtualId = result.empresaAtualId;

            console.log('SeletorEmpresa: Empresas encontradas:', this.empresas.length);
            console.log('SeletorEmpresa: Empresa atual ID:', this.empresaAtualId);

            // Se não tem empresas ou tem apenas uma, não mostrar seletor
            if (this.empresas.length <= 1) {
                console.log('SeletorEmpresa: Apenas uma empresa ou nenhuma, ocultando seletor');
                container.style.display = 'none';
                return;
            }

            // Mostrar seletor
            console.log('SeletorEmpresa: Mostrando seletor com', this.empresas.length, 'empresas');
            container.style.display = 'block';

            // Renderizar lista de empresas
            this.renderizarLista();
            this.atualizarEmpresaAtual();

        } catch (error) {
            console.error('SeletorEmpresa: Erro ao carregar empresas:', error);
            if (container) {
                container.style.display = 'none';
            }
        }
    }

    renderizarLista() {
        const lista = document.getElementById('lista-empresas');
        lista.innerHTML = '';

        if (this.empresas.length === 0) {
            lista.innerHTML = '<li class="dropdown-item-text text-muted">Nenhuma empresa disponível</li>';
            return;
        }

        this.empresas.forEach(empresa => {
            const item = document.createElement('li');
            const isSelecionada = empresa.id.toString() === this.empresaAtualId;

            item.innerHTML = `
                <a class="dropdown-item empresa-item ${isSelecionada ? 'empresa-selecionada' : ''}"
                   href="#"
                   data-empresa-id="${empresa.id}">
                    <div class="d-flex flex-column">
                        <div class="d-flex justify-content-between align-items-center">
                            <span class="empresa-item-nome">${this.escapeHtml(empresa.nome)}</span>
                            ${isSelecionada ? '<span class="empresa-item-badge"><i class="fas fa-check me-1"></i>Atual</span>' : ''}
                        </div>
                        ${empresa.cnpj ? `<span class="empresa-item-cnpj">${this.formatarCNPJ(empresa.cnpj)}</span>` : ''}
                    </div>
                </a>
            `;

            lista.appendChild(item);
        });
    }

    atualizarEmpresaAtual() {
        const empresaAtual = this.empresas.find(e => e.id.toString() === this.empresaAtualId);
        if (empresaAtual) {
            document.getElementById('empresa-atual-nome').textContent = empresaAtual.nome;
        }
    }

    setupEventListeners() {
        // Listener para cliques nos itens da lista
        document.getElementById('lista-empresas').addEventListener('click', async (e) => {
            e.preventDefault();

            const item = e.target.closest('.empresa-item');
            if (!item) return;

            const empresaId = parseInt(item.dataset.empresaId);

            // Se já está selecionada, não fazer nada
            if (empresaId.toString() === this.empresaAtualId) {
                return;
            }

            await this.trocarEmpresa(empresaId);
        });
    }

    async trocarEmpresa(empresaId) {
        try {
            // Mostrar loading
            const btnDropdown = document.getElementById('seletorEmpresaDropdown');
            const textoOriginal = btnDropdown.innerHTML;
            btnDropdown.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Trocando...';
            btnDropdown.disabled = true;

            const response = await fetch('/Login/TrocarEmpresa', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({ idEmpresaCliente: empresaId })
            });

            if (!response.ok) {
                throw new Error(`Erro ao trocar empresa: ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                // Sucesso - recarregar a página
                if (typeof showSuccess === 'function') {
                    showSuccess('Empresa alterada com sucesso! Recarregando...');
                }

                setTimeout(() => {
                    window.location.reload();
                }, 500);
            } else {
                throw new Error(result.message || 'Erro ao trocar empresa');
            }

        } catch (error) {
            console.error('Erro ao trocar empresa:', error);

            if (typeof showError === 'function') {
                showError('Erro ao trocar empresa: ' + error.message);
            }

            // Restaurar botão
            const btnDropdown = document.getElementById('seletorEmpresaDropdown');
            btnDropdown.innerHTML = textoOriginal;
            btnDropdown.disabled = false;
        }
    }

    formatarCNPJ(cnpj) {
        if (!cnpj) return '';
        // Remove tudo que não é número
        cnpj = cnpj.replace(/\D/g, '');

        // Formata: 00.000.000/0000-00
        if (cnpj.length === 14) {
            return cnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
        }

        return cnpj;
    }

    escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, m => map[m]);
    }
}

// Inicializar quando o DOM estiver pronto
const seletorEmpresa = new SeletorEmpresa();

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => seletorEmpresa.init());
} else {
    seletorEmpresa.init();
}

window.seletorEmpresa = seletorEmpresa;
