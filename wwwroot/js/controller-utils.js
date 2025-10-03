/*!
 * Controller Utilities - Funções genéricas para manipulação de controllers
 * 100% baseado em convenção - SEM mapeamentos manuais
 */

/**
 * Obtém o nome do controller baseado no nome da entidade
 * Convenção: NomeEntidade + "s" = NomeController
 * @param {string} entityName - Nome da entidade (ex: "Cliente", "Veiculo")
 * @returns {string} Nome do controller (ex: "Clientes", "Veiculos")
 */
function getControllerName(entityName) {
    if (!entityName) {
        console.error('Nome da entidade não pode ser vazio');
        return '';
    }

    // Convenção simples: Nome da entidade + "s"
    return entityName + 's';
}

/**
 * Obtém a URL completa para uma action de um controller
 * @param {string} entityName - Nome da entidade
 * @param {string} action - Action do controller (ex: "Index", "Details", "Edit")
 * @param {number|string} id - ID opcional do registro
 * @returns {string} URL completa (ex: "/Clientes/Details/123")
 */
function getActionUrl(entityName, action, id = null) {
    const controller = getControllerName(entityName);
    let url = `/${controller}/${action}`;

    if (id) {
        url += `/${id}`;
    }

    return url;
}

/**
 * Obtém a URL de detalhes de uma entidade
 * @param {string} entityName - Nome da entidade
 * @param {number|string} id - ID do registro
 * @returns {string} URL de detalhes (ex: "/Clientes/Details/123")
 */
function getDetailsUrl(entityName, id) {
    return getActionUrl(entityName, 'Details', id);
}

/**
 * Obtém a URL de edição de uma entidade
 * @param {string} entityName - Nome da entidade
 * @param {number|string} id - ID do registro
 * @returns {string} URL de edição (ex: "/Clientes/Edit/123")
 */
function getEditUrl(entityName, id) {
    return getActionUrl(entityName, 'Edit', id);
}

/**
 * Obtém a URL de criação de uma entidade
 * @param {string} entityName - Nome da entidade
 * @returns {string} URL de criação (ex: "/Clientes/Create")
 */
function getCreateUrl(entityName) {
    return getActionUrl(entityName, 'Create');
}

/**
 * Navega para a página de detalhes de uma entidade
 * @param {string} entityName - Nome da entidade
 * @param {number|string} id - ID do registro
 */
function navigateToDetails(entityName, id) {
    window.location.href = getDetailsUrl(entityName, id);
}

/**
 * Navega para a página de edição de uma entidade
 * @param {string} entityName - Nome da entidade
 * @param {number|string} id - ID do registro
 */
function navigateToEdit(entityName, id) {
    window.location.href = getEditUrl(entityName, id);
}

/**
 * Abre a página de detalhes em uma nova aba
 * @param {string} entityName - Nome da entidade
 * @param {number|string} id - ID do registro
 */
function openDetailsInNewTab(entityName, id) {
    window.open(getDetailsUrl(entityName, id), '_blank');
}

/**
 * Função legada: verEntidade (mantida para compatibilidade)
 * @deprecated Use navigateToDetails ou openDetailsInNewTab
 */
function verEntidade(entidade, id) {
    openDetailsInNewTab(entidade, id);
}

// Exportar para uso global
window.getControllerName = getControllerName;
window.getActionUrl = getActionUrl;
window.getDetailsUrl = getDetailsUrl;
window.getEditUrl = getEditUrl;
window.getCreateUrl = getCreateUrl;
window.navigateToDetails = navigateToDetails;
window.navigateToEdit = navigateToEdit;
window.openDetailsInNewTab = openDetailsInNewTab;
window.verEntidade = verEntidade;