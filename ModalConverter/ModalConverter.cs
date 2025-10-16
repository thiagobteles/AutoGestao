using System.Text.RegularExpressions;

partial class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🔧 CONVERSOR FINAL - CORREÇÕES ESPECÍFICAS INCLUÍDAS");
        Console.WriteLine("====================================================");
        
        Console.Write("Digite o caminho da pasta do projeto (ex: C:\\MeuProjeto): ");
        string projectPath = Console.ReadLine();
        
        if (!Directory.Exists(projectPath))
        {
            Console.WriteLine("❌ Pasta não encontrada!");
            Console.ReadKey();
            return;
        }
        
        Console.WriteLine("\n🔍 Processando arquivos...\n");
        
        // Processar Controllers (.cs)
        ProcessControllersComplete(projectPath);
        
        // Processar JavaScript (.js)
        ProcessJavaScriptComplete(projectPath);
        
        // Processar Views (.cshtml)
        ProcessViews(projectPath);
        
        // Processar Layouts (.cshtml)
        ProcessLayouts(projectPath);
        
        // Adicionar Response Handlers
        AddResponseHandlers(projectPath);
        
        // Adicionar correções específicas
        AddSpecificFixes(projectPath);
        
        Console.WriteLine("\n✅ CONVERSÃO FINAL CONCLUÍDA!");
        Console.WriteLine("📝 Problemas corrigidos:");
        Console.WriteLine("   ✓ Botões 'Limpar' com confirm() nativo");
        Console.WriteLine("   ✓ Grid duplo clique restaurado");
        Console.WriteLine("   ✓ Botões de ação reativados");
        Console.WriteLine("   ✓ Confirms em onclick substituídos");
        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
    
    static void ProcessControllersComplete(string basePath)
    {
        Console.WriteLine("📂 Processando Controllers (COMPLETO)...");
        
        string controllersPath = Path.Combine(basePath, "Controllers");
        if (!Directory.Exists(controllersPath))
        {
            Console.WriteLine("⚠️  Pasta Controllers não encontrada");
            return;
        }
        
        var csFiles = Directory.GetFiles(controllersPath, "*.cs", SearchOption.AllDirectories);
        
        foreach (string file in csFiles)
        {
            try
            {
                string content = File.ReadAllText(file);
                string newContent = content;
                bool changed = false;
                
                // 1. TempData Success/Error/SuccessMessage/ErrorMessage
                var tempDataPattern = @"TempData\[\""(Success|Error|SuccessMessage|ErrorMessage)\""\]\s*=\s*\""([^""]+)\"";";
                newContent = Regex.Replace(newContent, tempDataPattern, match =>
                {
                    string key = match.Groups[1].Value;
                    string message = match.Groups[2].Value;
                    string type = (key.Contains("Success")) ? "showSuccess" : "showError";
                    changed = true;
                    return $"TempData[\"NotificationScript\"] = \"{type}('{message}')\";";
                });
                
                // 2. JSON success/message básico
                var jsonBasicPattern = @"return Json\(new \{ success = (true|false), message = \""([^""]+)\""\s*\}\);";
                newContent = Regex.Replace(newContent, jsonBasicPattern, match =>
                {
                    string success = match.Groups[1].Value;
                    string message = match.Groups[2].Value;
                    string type = (success == "true") ? "showSuccess" : "showError";
                    changed = true;
                    return $"return Json(new {{ sucesso = {success}, mensagem = \"{message}\", script = \"{type}('{message}')\" }});";
                });
                
                // 3. JSON que já usa 'sucesso' mas não tem script
                var jsonSucessoPattern = @"return Json\(new \{ sucesso = (true|false), mensagem = \""([^""]+)\""\s*\}\);";
                newContent = Regex.Replace(newContent, jsonSucessoPattern, match =>
                {
                    string sucesso = match.Groups[1].Value;
                    string mensagem = match.Groups[2].Value;
                    string type = (sucesso == "true") ? "showSuccess" : "showError";
                    changed = true;
                    return $"return Json(new {{ sucesso = {sucesso}, mensagem = \"{mensagem}\", script = \"{type}('{mensagem}')\" }});";
                });
                
                // 4. TempData com interpolação de string
                var tempDataInterpolationPattern = @"TempData\[\""ErrorMessage\""\] = \$\""([^""]+)\{([^}]+)\}([^""]*)\""";
                newContent = Regex.Replace(newContent, tempDataInterpolationPattern, match =>
                {
                    string prefix = match.Groups[1].Value;
                    string variable = match.Groups[2].Value;
                    string suffix = match.Groups[3].Value;
                    changed = true;
                    return $"TempData[\"NotificationScript\"] = $\"showError('{prefix}{{{variable}}}{suffix}')\";";
                });
                
                if (changed)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($"✅ {Path.GetFileName(file)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em {Path.GetFileName(file)}: {ex.Message}");
            }
        }
    }
    
    static void ProcessJavaScriptComplete(string basePath)
    {
        Console.WriteLine("\n📂 Processando JavaScript (COMPLETO)...");
        
        string jsPath = Path.Combine(basePath, "wwwroot", "js");
        if (!Directory.Exists(jsPath))
        {
            Console.WriteLine("⚠️  Pasta wwwroot/js não encontrada");
            return;
        }
        
        var jsFiles = Directory.GetFiles(jsPath, "*.js", SearchOption.AllDirectories);
        
        foreach (string file in jsFiles)
        {
            try
            {
                string content = File.ReadAllText(file);
                string newContent = content;
                bool changed = false;
                
                // 1. showToast básico
                var showToastPattern = @"showToast\(\s*['""]([^'""]+)['""],\s*['""](success|error|warning|info)['""]\s*\)";
                newContent = Regex.Replace(newContent, showToastPattern, match =>
                {
                    string message = match.Groups[1].Value;
                    string type = match.Groups[2].Value;
                    string newFunction = type switch
                    {
                        "success" => "showSuccess",
                        "error" => "showError", 
                        "warning" => "showWarning",
                        _ => "showInfo"
                    };
                    changed = true;
                    return $"{newFunction}('{message}')";
                });
                
                // 2. this.showToast
                var thisShowToastPattern = @"this\.showToast\s*\(\s*['""]([^'""]+)['""],\s*['""](success|error|warning|info)['""]\s*\)";
                newContent = Regex.Replace(newContent, thisShowToastPattern, match =>
                {
                    string message = match.Groups[1].Value;
                    string type = match.Groups[2].Value;
                    string newFunction = type switch
                    {
                        "success" => "showSuccess",
                        "error" => "showError",
                        "warning" => "showWarning", 
                        _ => "showInfo"
                    };
                    changed = true;
                    return $"{newFunction}('{message}')";
                });
                
                // 3. confirm() para exclusão
                var confirmDeletePattern = @"if\s*\(\s*!confirm\s*\(\s*['""]([^'""]*(?:excluir|delete)[^'""]*)['""\s]*\)\s*\)";
                newContent = Regex.Replace(newContent, confirmDeletePattern, match =>
                {
                    changed = true;
                    return "const confirmed = await confirmDelete(); if (!confirmed)";
                }, RegexOptions.IgnoreCase);
                
                // 4. confirm() genérico
                var confirmGenericPattern = @"if\s*\(\s*confirm\s*\(\s*['""]([^'""]+)['""\s]*\)\s*\)";
                newContent = Regex.Replace(newContent, confirmGenericPattern, match =>
                {
                    string message = match.Groups[1].Value;
                    changed = true;
                    return $"const confirmed = await showConfirm('{message}'); if (confirmed)";
                });
                
                // 5. alert() simples
                var alertPattern = @"alert\s*\(\s*['""]([^'""]+)['""\s]*\)";
                newContent = Regex.Replace(newContent, alertPattern, match =>
                {
                    string message = match.Groups[1].Value;
                    changed = true;
                    return $"showInfo('{message}')";
                });
                
                if (changed)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($"✅ {Path.GetFileName(file)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em {Path.GetFileName(file)}: {ex.Message}");
            }
        }
    }
    
    static void ProcessViews(string basePath)
    {
        Console.WriteLine("\n📂 Processando Views (.cshtml)...");
        
        string viewsPath = Path.Combine(basePath, "Views");
        if (!Directory.Exists(viewsPath))
        {
            Console.WriteLine("⚠️  Pasta Views não encontrada");
            return;
        }
        
        var cshtmlFiles = Directory.GetFiles(viewsPath, "*.cshtml", SearchOption.AllDirectories);
        
        foreach (string file in cshtmlFiles)
        {
            try
            {
                string content = File.ReadAllText(file);
                string newContent = content;
                bool changed = false;
                
                // onclick com confirm - padrão geral
                var onclickConfirmPattern = @"onclick\s*=\s*['""]([^'""]*confirm\([^'""]+\)[^'""]*)['""]";
                newContent = Regex.Replace(newContent, onclickConfirmPattern, match =>
                {
                    string originalOnclick = match.Groups[1].Value;
                    changed = true;
                    return $"data-confirm-action=\"{originalOnclick.Replace("\"", "&quot;")}\" onclick=\"handleConfirmClick(this, event)\"";
                });
                
                if (changed)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($"✅ {Path.GetFileName(file)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em {Path.GetFileName(file)}: {ex.Message}");
            }
        }
    }
    
    static void ProcessLayouts(string basePath)
    {
        Console.WriteLine("\n📂 Processando Layouts...");
        
        string viewsPath = Path.Combine(basePath, "Views", "Shared");
        if (!Directory.Exists(viewsPath))
        {
            Console.WriteLine("⚠️  Pasta Views/Shared não encontrada");
            return;
        }
        
        var layoutFiles = Directory.GetFiles(viewsPath, "_Layout*.cshtml");
        
        foreach (string file in layoutFiles)
        {
            try
            {
                string content = File.ReadAllText(file);
                string newContent = content;
                bool changed = false;
                
                if (!content.Contains("complete-modal-system.js"))
                {
                    var bodyClosePattern = @"(\s*</body>)";
                    if (Regex.IsMatch(content, bodyClosePattern))
                    {
                        newContent = Regex.Replace(newContent, bodyClosePattern, match =>
                        {
                            changed = true;
                            return $@"
    <!-- Sistema de Modal de Notificação -->
    <link rel=""stylesheet"" href=""~/css/complete-modal-styles.css"" />
    <script src=""~/js/complete-modal-system.js""></script>
    <script src=""~/js/response-handlers.js""></script>
    <script src=""~/js/grid-specific-fixes.js""></script>

    <!-- Processar TempData NotificationScript -->
    @if (TempData[""NotificationScript""] != null)
    {{
        <script>
            document.addEventListener('DOMContentLoaded', function() {{
                setTimeout(function() {{
                    @Html.Raw(TempData[""NotificationScript""])
                }}, 500);
            }});
        </script>
    }}

{match.Groups[1].Value}";
                        });
                    }
                }
                else if (!content.Contains("grid-specific-fixes.js"))
                {
                    var responseHandlerPattern = @"(<script src=""~/js/response-handlers\.js""></script>)";
                    if (Regex.IsMatch(content, responseHandlerPattern))
                    {
                        newContent = Regex.Replace(newContent, responseHandlerPattern, match =>
                        {
                            changed = true;
                            return match.Groups[1].Value + "\n    <script src=\"~/js/grid-specific-fixes.js\"></script>";
                        });
                    }
                }
                
                if (changed)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($"✅ {Path.GetFileName(file)}");
                }
                else
                {
                    Console.WriteLine($"ℹ️  {Path.GetFileName(file)} - já atualizado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro em {Path.GetFileName(file)}: {ex.Message}");
            }
        }
    }
    
    static void AddResponseHandlers(string basePath)
    {
        Console.WriteLine("\n📂 Adicionando Response Handlers...");
        
        string jsPath = Path.Combine(basePath, "wwwroot", "js");
        if (!Directory.Exists(jsPath))
        {
            Console.WriteLine("⚠️  Pasta wwwroot/js não encontrada");
            return;
        }
        
        string responseHandlersPath = Path.Combine(jsPath, "response-handlers.js");
        
        if (File.Exists(responseHandlersPath))
        {
            Console.WriteLine($"ℹ️  response-handlers.js já existe");
            return;
        }
        
        string responseHandlersContent = @"/**
 * SISTEMA DE HANDLERS AUTOMÁTICOS PARA RESPOSTAS JSON
 */

class ResponseHandler {
    constructor() {
        this.init();
    }

    init() {
        this.interceptFetch();
        console.log('✅ Sistema de handlers de resposta inicializado');
    }

    interceptFetch() {
        const originalFetch = window.fetch;
        
        window.fetch = async (...args) => {
            try {
                const response = await originalFetch(...args);
                const clonedResponse = response.clone();
                
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    try {
                        const data = await clonedResponse.json();
                        if (data.script) {
                            this.executeScript(data.script);
                        }
                    } catch (e) {
                        // Ignorar erros de parsing JSON
                    }
                }
                
                return response;
            } catch (error) {
                console.error('Erro no fetch:', error);
                throw error;
            }
        };
    }

    executeScript(script) {
        try {
            if (typeof window.modalSystem !== 'undefined') {
                eval(script);
            } else {
                setTimeout(() => eval(script), 500);
            }
        } catch (error) {
            console.error('Erro ao executar script:', error);
        }
    }
}

window.responseHandler = new ResponseHandler();

// Substituir confirmarExclusao global
window.confirmarExclusao = async function(id) {
    const confirmed = await confirmDelete();
    
    if (confirmed) {
        const currentPath = window.location.pathname.toLowerCase();
        let controller = '';

        if (currentPath.includes('veiculos')) controller = 'Veiculos';
        else if (currentPath.includes('clientes')) controller = 'Clientes';
        else if (currentPath.includes('vendedores')) controller = 'Vendedores';
        else if (currentPath.includes('fornecedores')) controller = 'Fornecedores';

        if (controller) {
            window.showLoading && window.showLoading(true);
            
            try {
                const response = await fetch(`/${controller}/Delete/${id}`, {
                    method: 'POST'
                });
                const result = await response.json();
                
                if (result.sucesso) {
                    await showSuccess('Registro excluído com sucesso!');
                    setTimeout(() => window.location.reload(), 2000);
                } else {
                    await showError(result.mensagem || 'Erro ao excluir registro');
                }
            } catch (error) {
                await showError('Erro de conexão');
            } finally {
                window.showLoading && window.showLoading(false);
            }
        }
    }
};

console.log('🔗 Sistema de handlers automáticos carregado');";
        
        try
        {
            File.WriteAllText(responseHandlersPath, responseHandlersContent);
            Console.WriteLine($"✅ response-handlers.js criado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao criar response-handlers.js: {ex.Message}");
        }
    }
    
    static void AddSpecificFixes(string basePath)
    {
        Console.WriteLine("\n📂 Adicionando Correções Específicas...");
        
        string jsPath = Path.Combine(basePath, "wwwroot", "js");
        if (!Directory.Exists(jsPath))
        {
            Console.WriteLine("⚠️  Pasta wwwroot/js não encontrada");
            return;
        }
        
        string specificFixesPath = Path.Combine(jsPath, "grid-specific-fixes.js");
        
        if (File.Exists(specificFixesPath))
        {
            Console.WriteLine($"ℹ️  grid-specific-fixes.js já existe");
            return;
        }
        
        string specificFixesContent = @"/**
 * CORREÇÕES ESPECÍFICAS PARA PROBLEMAS DA GRID
 */

// Interceptar cliques para substituir confirms
document.addEventListener('click', async function(e) {
    const target = e.target;
    
    // Botões com confirm() em onclick
    if (target.hasAttribute('onclick') && target.getAttribute('onclick').includes('confirm')) {
        const onclick = target.getAttribute('onclick');
        e.preventDefault();
        e.stopPropagation();
        "+ @$"
        const confirmMatch = onclick.match(/confirm\(['\""]([^'\""]*)['\""]\)/);" + @"
        const message = confirmMatch ? confirmMatch[1] : 'Tem certeza que deseja continuar?';
        
        const confirmed = await showConfirm(message, {
            title: 'Confirmação',
            okText: 'Confirmar',
            cancelText: 'Cancelar',
            type: 'warning'
        });
        
        if (confirmed) {
            const cleanCode = onclick.replace(/confirm\([^)]+\)\s*&&?\s*/, '');
            try {
                eval(cleanCode);
            } catch (error) {
                console.error('Erro ao executar código:', error);
            }
        }
    }
    
    // Botões de limpar específicos
    if (target.textContent && target.textContent.toLowerCase().includes('limpar')) {
        if (target.hasAttribute('onclick')) {
            const onclick = target.getAttribute('onclick');
            if (onclick.includes('confirm')) {
                e.preventDefault();
                
                const confirmed = await showConfirm('Tem certeza que deseja limpar todos os campos?', {
                    title: 'Limpar Formulário',
                    okText: 'Sim, Limpar',
                    cancelText: 'Cancelar',
                    type: 'warning'
                });
                
                if (confirmed) {
                    const cleanCode = onclick.replace(/confirm\([^)]+\)\s*&&?\s*/, '');
                    eval(cleanCode);
                }
            }
        }
    }
});

// Restaurar duplo clique na grid
function restoreGridDoubleClick() {
    const gridTables = document.querySelectorAll('.base-grid-table tbody tr');
    
    gridTables.forEach(row => {
        row.addEventListener('dblclick', function(e) {
            if (e.target.closest('.dropdown, .btn, button, a')) return;
            
            const firstCell = row.querySelector('td:first-child');
            const entityId = firstCell ? firstCell.textContent.trim() : null;
            
            const currentPath = window.location.pathname;
            let controller = '';
            
            if (currentPath.includes('Clientes')) controller = 'Clientes';
            else if (currentPath.includes('Veiculos')) controller = 'Veiculos';
            else if (currentPath.includes('Vendedores')) controller = 'Vendedores';
            else if (currentPath.includes('Fornecedores')) controller = 'Fornecedores';
            
            if (controller && entityId && !isNaN(entityId)) {
                window.location.href = `/${controller}/Edit/${entityId}`;
            }
        });
        
        row.style.cursor = 'pointer';
        row.title = 'Clique duplo para editar';
    });
}

// Executar quando DOM carregar
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(function() {
        restoreGridDoubleClick();
        
        // Reativar dropdowns Bootstrap" + $@"
        document.querySelectorAll('[data-bs-toggle=\""dropdown\""]').forEach(button => " + @"{
            try {
                new bootstrap.Dropdown(button);
            } catch (e) {
                // Ignorar se Bootstrap não disponível
            }
        });
        
        console.log('✅ Correções específicas aplicadas');
    }, 1000);
});

console.log('🛠️ Sistema de correções específicas carregado');";
        
        try
        {
            File.WriteAllText(specificFixesPath, specificFixesContent);
            Console.WriteLine($"✅ grid-specific-fixes.js criado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro ao criar grid-specific-fixes.js: {ex.Message}");
        }
    }
}