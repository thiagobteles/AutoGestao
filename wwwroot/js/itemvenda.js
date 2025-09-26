// Função para buscar informações do produto quando selecionado
function onProdutoChange(select) {
    const produtoId = select.value;
    if (!produtoId) return;

    fetch(`/ItensVenda/GetProdutoInfo?produtoId=${produtoId}`)
        .then(response => response.json())
        .then(data => {
            // Preencher campo de valor unitário com o preço de venda
            const valorUnitarioField = document.querySelector('[name="ValorUnitario"]');
            if (valorUnitarioField) {
                valorUnitarioField.value = data.precoVenda;
            }

            // Mostrar informações do estoque
            showEstoqueInfo(data.estoqueAtual, data.estoqueMinimo);

            // Recalcular valores
            recalcularValores();
        })
        .catch(error => {
            console.error('Erro ao buscar informações do produto:', error);
        });
}

function recalcularValores() {
    const quantidade = parseFloat(document.querySelector('[name="Quantidade"]').value || 0);
    const valorUnitario = parseFloat(document.querySelector('[name="ValorUnitario"]').value || 0);
    const percentualDesconto = parseFloat(document.querySelector('[name="PercentualDesconto"]').value || 0);

    const subtotal = quantidade * valorUnitario;
    const valorDesconto = subtotal * (percentualDesconto / 100);
    const valorTotal = subtotal - valorDesconto;

    // Atualizar campos calculados
    const valorDescontoField = document.querySelector('[name="ValorDesconto"]');
    const valorTotalField = document.querySelector('[name="ValorTotal"]');

    if (valorDescontoField) valorDescontoField.value = valorDesconto.toFixed(2);
    if (valorTotalField) valorTotalField.value = valorTotal.toFixed(2);
}

function showEstoqueInfo(estoqueAtual, estoqueMinimo) {
    const quantidadeField = document.querySelector('[name="Quantidade"]');
    if (!quantidadeField) return;

    // Remover avisos anteriores
    const existingAlert = quantidadeField.parentNode.querySelector('.estoque-alert');
    if (existingAlert) existingAlert.remove();

    // Criar aviso de estoque
    const alert = document.createElement('div');
    alert.className = 'estoque-alert mt-1 small';

    if (estoqueAtual <= estoqueMinimo) {
        alert.className += ' text-warning';
        alert.innerHTML = `<i class="fas fa-exclamation-triangle"></i> Estoque baixo: ${estoqueAtual} unidades`;
    } else {
        alert.className += ' text-info';
        alert.innerHTML = `<i class="fas fa-info-circle"></i> Estoque disponível: ${estoqueAtual} unidades`;
    }

    quantidadeField.parentNode.appendChild(alert);

    // Definir máximo do campo quantidade
    quantidadeField.setAttribute('max', estoqueAtual);
}

// Event listeners
document.addEventListener('DOMContentLoaded', function () {
    const produtoSelect = document.querySelector('[name="ProdutoId"]');
    if (produtoSelect) {
        produtoSelect.addEventListener('change', (e) => onProdutoChange(e.target));
    }

    const quantidadeField = document.querySelector('[name="Quantidade"]');
    const valorUnitarioField = document.querySelector('[name="ValorUnitario"]');
    const descontoField = document.querySelector('[name="PercentualDesconto"]');

    [quantidadeField, valorUnitarioField, descontoField].forEach(field => {
        if (field) {
            field.addEventListener('input', recalcularValores);
        }
    });
});