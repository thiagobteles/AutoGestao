# 📊 Sistema de Relatórios Dinâmicos - AutoGestão

## 🎯 Visão Geral

Sistema completo de geração e gerenciamento de relatórios personalizados com interface visual drag-and-drop.

## ✨ Funcionalidades Principais

### 1. **Designer Visual de Templates**
- Interface drag-and-drop intuitiva
- Construção visual sem necessidade de código
- Preview em tempo real
- Suporte para múltiplas entidades

### 2. **Tipos de Layout**

#### **Grid Layout** (Layout em Colunas)
- Organize campos em grid de 1 a 4 colunas
- Ideal para dados estruturados
- Campos lado a lado

#### **Row Layout** (Linha Completa)
- Cada campo ocupa uma linha inteira
- Ótimo para campos longos ou destaque
- Label à esquerda, valor à direita

#### **Table Layout** (Tabela)
- Para listas de itens relacionados
- Suporta totalizadores automáticos
- Colunas customizáveis

### 3. **Design Elegante**
- Gradientes modernos
- Sombras suaves
- Tipografia profissional
- Responsivo e otimizado para impressão
- Cores personalizáveis por seção

## 🚀 Como Usar

### Criar um Novo Template

1. **Acesse o Menu**
   ```
   Relatórios > Templates de Relatórios > Criar Novo Template
   ```

2. **Selecione a Entidade**
   - Escolha a entidade base (Veiculo, Cliente, etc.)
   - O sistema carregará automaticamente todos os campos disponíveis

3. **Construa as Seções**
   - Clique em "Adicionar Seção"
   - Defina o título da seção
   - Escolha o tipo de layout (Grid, Row ou Table)
   - Se Grid, escolha o número de colunas (1-4)

4. **Adicione Campos**
   - Arraste campos da lista à esquerda
   - Solte na área da seção desejada
   - Os campos são organizados automaticamente

5. **Configure**
   - Nome do template
   - Descrição (opcional)
   - Marcar como padrão (opcional)

6. **Preview e Salvar**
   - Clique em "Visualizar Preview" para ver o resultado
   - Clique em "Salvar Template" para persistir

### Gerar um Relatório

```javascript
// Usando o helper JavaScript
ReportHelper.quickReport('Veiculo', 123);

// Ou via template salvo
ReportHelper.fromTemplate(templateId, entityId);
```

## 📁 Estrutura de Arquivos

```
AutoGestao/
├── Controllers/
│   ├── Base/
│   │   ├── ReportController.cs          # Controller de geração de relatórios
│   │   └── ReportTemplateController.cs  # CRUD de templates
│   └── ReportBuilderController.cs       # Builder visual
├── Services/
│   ├── ReportService.cs                 # Serviço de geração de HTML
│   └── EntityInspectorService.cs        # Descoberta de propriedades
├── Models/
│   └── Report/
│       └── ReportTemplate.cs            # Modelo de template
├── Views/
│   ├── ReportBuilder/
│   │   └── Create.cshtml                # Interface do builder
│   └── ReportTemplate/
│       └── Index.cshtml                 # Lista de templates
└── wwwroot/
    └── js/
        ├── report-helper.js             # Helper de relatórios
        └── report-builder.js            # Lógica do builder visual
```

## 🎨 Customização de Estilos

### Cores do Tema
O relatório usa um gradiente roxo/azul por padrão:
```css
--primary: #667eea;
--secondary: #764ba2;
```

Para alterar, edite em `ReportController.cs > GetReportStyles()`:
```css
background: linear-gradient(135deg, #SUA_COR_1 0%, #SUA_COR_2 100%);
```

### Fontes
Por padrão usa:
```css
font-family: 'Segoe UI', 'Helvetica Neue', Arial, sans-serif;
```

## 🔧 API Endpoints

### ReportBuilder

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/ReportBuilder/Create` | Criar novo template |
| GET | `/ReportBuilder/Edit/{id}` | Editar template |
| GET | `/ReportBuilder/GetEntities` | Listar entidades disponíveis |
| GET | `/ReportBuilder/GetEntityProperties?entityName=X` | Propriedades da entidade |
| POST | `/ReportBuilder/Save` | Salvar template |
| POST | `/ReportBuilder/Preview` | Gerar preview |

### Report

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Report/Quick?entityType=X&entityId=Y` | Relatório rápido |
| GET | `/Report/GenerateFromSaved?templateId=X&entityId=Y` | Usar template salvo |
| POST | `/Report/Generate` | Relatório com template custom |

## 📦 Modelo de Dados

### ReportTemplate
```csharp
{
  "name": "Nome do Template",
  "sections": [
    {
      "title": "Título da Seção",
      "type": "grid",        // grid, row ou table
      "columns": 3,          // 1-4 para grid
      "order": 0,
      "fields": [
        {
          "label": "Label do Campo",
          "propertyName": "NomePropriedade",
          "format": "dd/MM/yyyy",  // Opcional
          "order": 0,
          "displayType": "default", // default, badge, highlight
          "bold": false,
          "columnSpan": 1         // Para ocupar mais colunas
        }
      ]
    }
  ]
}
```

## 🎯 Propriedades Suportadas

### Tipos de Dados
- ✅ Texto (string)
- ✅ Número (int, long)
- ✅ Moeda (decimal)
- ✅ Data (DateTime)
- ✅ Booleano (bool)
- ✅ Enumeradores
- ✅ Propriedades Navegacionais (ex: `VeiculoMarca.Descricao`)

### Formatos
- **Data**: `dd/MM/yyyy`, `dd/MM/yyyy HH:mm`
- **Moeda**: `C2` (formato moeda com 2 casas)
- **Número**: `N0`, `N2`, etc.

## 🌟 Exemplos de Uso

### Exemplo 1: Relatório de Veículo Completo
```javascript
const template = {
  name: "Relatório Completo de Veículo",
  sections: [
    {
      title: "Dados do Veículo",
      type: "grid",
      columns: 3,
      fields: [
        { label: "Marca", propertyName: "VeiculoMarca.Descricao" },
        { label: "Modelo", propertyName: "VeiculoMarcaModelo.Descricao" },
        { label: "Placa", propertyName: "Placa" },
        { label: "Ano", propertyName: "AnoFabricacao" },
        { label: "Cor", propertyName: "VeiculoCor.Descricao" },
        { label: "Preço", propertyName: "PrecoVenda", format: "C2" }
      ]
    },
    {
      title: "Proprietário",
      type: "row",
      fields: [
        { label: "Nome", propertyName: "Cliente.Nome", bold: true },
        { label: "CPF/CNPJ", propertyName: "Cliente.Documento" },
        { label: "Telefone", propertyName: "Cliente.Telefone" }
      ]
    }
  ]
};
```

### Exemplo 2: Usando no Código
```csharp
// No controller
public async Task<IActionResult> ImprimirVeiculo(long id)
{
    var veiculo = await _context.Veiculos
        .Include(v => v.VeiculoMarca)
        .Include(v => v.VeiculoMarcaModelo)
        .Include(v => v.Cliente)
        .FirstOrDefaultAsync(v => v.Id == id);

    var template = await GetTemplateByName("Relatório Completo de Veículo");
    var html = _reportService.GenerateReportHtml(veiculo, template);

    return Content(html, "text/html");
}
```

## 💡 Dicas e Boas Práticas

1. **Organize por Seções Lógicas**
   - Agrupe campos relacionados
   - Use títulos descritivos

2. **Escolha o Layout Adequado**
   - Grid: Dados tabulares, múltiplos campos curtos
   - Row: Campos longos, destaque individual
   - Table: Listas de itens

3. **Use Propriedades Navegacionais**
   - `VeiculoMarca.Descricao` em vez de `IdVeiculoMarca`
   - Mais legível e profissional

4. **Preview Sempre**
   - Teste antes de salvar
   - Verifique com dados reais

5. **Templates Reutilizáveis**
   - Crie templates genéricos
   - Clone e customize quando necessário

## 🐛 Troubleshooting

### Erro: "Propriedade não encontrada"
- Verifique se a entidade tem Include() para propriedades navegacionais
- Confirme o nome da propriedade (case-sensitive)

### Relatório não imprime cores
- Adicione `-webkit-print-color-adjust: exact` no CSS
- Já está implementado no sistema

### Campo não aparece
- Verifique se a propriedade tem valor
- Campos nulos mostram "-"

## 📞 Suporte

Para dúvidas ou sugestões sobre o sistema de relatórios, entre em contato com a equipe de desenvolvimento.

---

**Versão**: 1.0
**Data**: Novembro 2024
**Autor**: Sistema AutoGestão
