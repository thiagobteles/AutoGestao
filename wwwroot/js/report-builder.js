/**
 * Report Template Builder
 * Sistema de construção visual de templates de relatórios
 */

const TemplateBuilder = {
    currentEntity: null,
    entityProperties: [],
    sections: [],
    templateId: null,

    /**
     * Inicializar o builder
     */
    async init(initialData = {}) {
        console.log('🚀 Inicializando Template Builder...');

        this.templateId = initialData.templateId;

        // Carregar entidades disponíveis
        await this.loadEntities();

        // Se tem entityType inicial, selecionar
        if (initialData.entityType) {
            document.getElementById('entitySelector').value = initialData.entityType;
            await this.selectEntity(initialData.entityType);
        }

        // Se tem template JSON, carregar
        if (initialData.templateJson) {
            this.loadTemplate(initialData.templateJson);
        }

        // Setup event listeners
        this.setupEventListeners();

        console.log('✅ Template Builder inicializado');
    },

    /**
     * Configurar event listeners
     */
    setupEventListeners() {
        // Seletor de entidade
        document.getElementById('entitySelector').addEventListener('change', (e) => {
            this.selectEntity(e.target.value);
        });
    },

    /**
     * Carregar lista de entidades
     */
    async loadEntities() {
        try {
            const response = await fetch('/ReportBuilder/GetEntities');
            const result = await response.json();

            if (result.success) {
                const selector = document.getElementById('entitySelector');
                selector.innerHTML = '<option value="">Selecione uma entidade...</option>';

                result.data.forEach(entity => {
                    const option = document.createElement('option');
                    option.value = entity.name;
                    option.textContent = `${entity.displayName}`;
                    option.setAttribute('data-icon', entity.icon);
                    selector.appendChild(option);
                });
            } else {
                this.showError('Erro ao carregar entidades: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao carregar entidades:', error);
            this.showError('Erro ao carregar entidades');
        }
    },

    /**
     * Selecionar entidade e carregar suas propriedades
     */
    async selectEntity(entityName) {
        if (!entityName) return;

        try {
            const response = await fetch(`/ReportBuilder/GetEntityProperties?entityName=${entityName}`);
            const result = await response.json();

            if (result.success) {
                this.currentEntity = result.data;
                this.entityProperties = result.data.properties;
                this.renderProperties();
            } else {
                this.showError('Erro ao carregar propriedades: ' + result.message);
            }
        } catch (error) {
            console.error('Erro ao carregar propriedades:', error);
            this.showError('Erro ao carregar propriedades da entidade');
        }
    },

    /**
     * Renderizar lista de propriedades
     */
    renderProperties() {
        const container = document.getElementById('propertiesList');
        container.innerHTML = '';

        // Agrupar por seção
        const grouped = {};
        this.entityProperties.forEach(prop => {
            if (!grouped[prop.section]) {
                grouped[prop.section] = [];
            }
            grouped[prop.section].push(prop);
        });

        // Renderizar grupos
        for (const [section, properties] of Object.entries(grouped)) {
            const groupDiv = document.createElement('div');
            groupDiv.className = 'property-group';

            const titleDiv = document.createElement('div');
            titleDiv.className = 'property-group-title';
            titleDiv.innerHTML = `<i class="fas fa-folder"></i> ${section}`;
            groupDiv.appendChild(titleDiv);

            properties.forEach(prop => {
                const propDiv = document.createElement('div');
                propDiv.className = 'property-item';
                propDiv.draggable = true;
                propDiv.setAttribute('data-property', JSON.stringify(prop));

                propDiv.innerHTML = `
                    <i class="${prop.icon}"></i>
                    <span class="property-label">${prop.label}</span>
                    <span class="property-type">${prop.propertyType}</span>
                `;

                // Drag events
                propDiv.addEventListener('dragstart', (e) => {
                    e.dataTransfer.setData('application/json', JSON.stringify(prop));
                    e.dataTransfer.effectAllowed = 'copy';
                });

                groupDiv.appendChild(propDiv);
            });

            container.appendChild(groupDiv);
        }
    },

    /**
     * Adicionar nova seção
     */
    addSection() {
        const section = {
            id: 'section-' + Date.now(),
            title: 'Nova Seção',
            type: 'grid',
            columns: 3,
            order: this.sections.length,
            fields: [],
            icon: 'fas fa-folder'
        };

        this.sections.push(section);
        this.renderSections();
    },

    /**
     * Renderizar seções no canvas
     */
    renderSections() {
        const canvas = document.getElementById('builderCanvas');

        if (this.sections.length === 0) {
            canvas.innerHTML = `
                <div class="empty-state">
                    <i class="fas fa-file-alt"></i>
                    <p>Clique em "Adicionar Seção" para começar a construir seu relatório</p>
                </div>
            `;
            return;
        }

        canvas.innerHTML = '';

        this.sections.forEach((section, index) => {
            const sectionDiv = document.createElement('div');
            sectionDiv.className = 'section-container';
            sectionDiv.setAttribute('data-section-id', section.id);

            sectionDiv.innerHTML = `
                <div class="section-header">
                    <input type="text"
                           class="section-title-input"
                           value="${section.title}"
                           onchange="TemplateBuilder.updateSectionTitle('${section.id}', this.value)"
                           placeholder="Título da seção">
                    <div class="section-controls">
                        <select class="section-control-btn" onchange="TemplateBuilder.changeSectionType('${section.id}', this.value)">
                            <option value="grid" ${section.type === 'grid' ? 'selected' : ''}>Grid</option>
                            <option value="row" ${section.type === 'row' ? 'selected' : ''}>Linha</option>
                            <option value="table" ${section.type === 'table' ? 'selected' : ''}>Tabela</option>
                            <option value="richtext" ${section.type === 'richtext' ? 'selected' : ''}>Texto Livre</option>
                            <option value="external_query" ${section.type === 'external_query' ? 'selected' : ''}>Consulta Externa</option>
                        </select>
                        ${section.type === 'grid' ? `
                            <select class="section-control-btn" onchange="TemplateBuilder.changeSectionColumns('${section.id}', this.value)">
                                ${[1,2,3,4].map(n => `<option value="${n}" ${section.columns === n ? 'selected' : ''}>${n} col</option>`).join('')}
                            </select>
                        ` : ''}
                        <button class="section-control-btn" onclick="TemplateBuilder.moveSection('${section.id}', 'up')" ${index === 0 ? 'disabled' : ''}>
                            <i class="fas fa-arrow-up"></i>
                        </button>
                        <button class="section-control-btn" onclick="TemplateBuilder.moveSection('${section.id}', 'down')" ${index === this.sections.length - 1 ? 'disabled' : ''}>
                            <i class="fas fa-arrow-down"></i>
                        </button>
                        <button class="section-control-btn danger" onclick="TemplateBuilder.removeSection('${section.id}')">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
                ${section.type === 'richtext' ? `
                    <div class="richtext-editor-container" data-section-id="${section.id}">
                        <div class="richtext-toolbar">
                            <button type="button" onclick="TemplateBuilder.execCommand('bold')" title="Negrito">
                                <i class="fas fa-bold"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('italic')" title="Itálico">
                                <i class="fas fa-italic"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('underline')" title="Sublinhado">
                                <i class="fas fa-underline"></i>
                            </button>
                            <span class="toolbar-separator"></span>
                            <button type="button" onclick="TemplateBuilder.execCommand('justifyLeft')" title="Alinhar à esquerda">
                                <i class="fas fa-align-left"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('justifyCenter')" title="Centralizar">
                                <i class="fas fa-align-center"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('justifyRight')" title="Alinhar à direita">
                                <i class="fas fa-align-right"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('justifyFull')" title="Justificar">
                                <i class="fas fa-align-justify"></i>
                            </button>
                            <span class="toolbar-separator"></span>
                            <button type="button" onclick="TemplateBuilder.execCommand('insertUnorderedList')" title="Lista não ordenada">
                                <i class="fas fa-list-ul"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('insertOrderedList')" title="Lista ordenada">
                                <i class="fas fa-list-ol"></i>
                            </button>
                            <span class="toolbar-separator"></span>
                            <button type="button" onclick="TemplateBuilder.execCommand('indent')" title="Aumentar recuo">
                                <i class="fas fa-indent"></i>
                            </button>
                            <button type="button" onclick="TemplateBuilder.execCommand('outdent')" title="Diminuir recuo">
                                <i class="fas fa-outdent"></i>
                            </button>
                            <span class="toolbar-separator"></span>
                            <select onchange="TemplateBuilder.changeTextColor(this.value, '${section.id}')" title="Cor do texto">
                                <option value="">Cor do texto</option>
                                <option value="#000000">Preto</option>
                                <option value="#FF0000">Vermelho</option>
                                <option value="#00FF00">Verde</option>
                                <option value="#0000FF">Azul</option>
                                <option value="#FFFF00">Amarelo</option>
                                <option value="#FF00FF">Magenta</option>
                                <option value="#00FFFF">Ciano</option>
                            </select>
                            <select onchange="TemplateBuilder.changeFontSize(this.value, '${section.id}')" title="Tamanho da fonte">
                                <option value="">Tamanho</option>
                                <option value="1">Muito pequeno</option>
                                <option value="2">Pequeno</option>
                                <option value="3">Normal</option>
                                <option value="4">Médio</option>
                                <option value="5">Grande</option>
                                <option value="6">Muito grande</option>
                                <option value="7">Enorme</option>
                            </select>
                            <span class="toolbar-separator"></span>
                            <button type="button" onclick="TemplateBuilder.insertImage('${section.id}')" title="Inserir imagem">
                                <i class="fas fa-image"></i>
                            </button>
                        </div>
                        <div class="richtext-editor"
                             id="richtext-${section.id}"
                             contenteditable="true"
                             data-section-id="${section.id}"
                             onblur="TemplateBuilder.saveRichTextContent('${section.id}')"
                             style="min-height: 200px; padding: 15px; background: white; border: 2px solid #dee2e6; border-radius: 8px; outline: none;">
                            ${section.richTextContent || '<p>Digite seu texto aqui...</p>'}
                        </div>
                    </div>
                ` : section.type === 'external_query' ? `
                    <div class="external-query-container" data-section-id="${section.id}">
                        <div class="query-editor-header">
                            <i class="fas fa-database"></i>
                            <span>Consulta SQL Personalizada</span>
                            <small style="color: #6c757d; margin-left: auto;">Use @Id para referenciar o ID da entidade</small>
                        </div>
                        <textarea
                            class="query-editor"
                            id="query-${section.id}"
                            data-section-id="${section.id}"
                            onblur="TemplateBuilder.saveQueryContent('${section.id}')"
                            placeholder="Digite sua consulta SQL aqui...&#10;Exemplo:&#10;SELECT Nome, Email, Telefone &#10;FROM Clientes &#10;WHERE Id = @Id"
                            rows="10">${section.sqlQuery || ''}</textarea>
                        <div class="query-help">
                            <div class="query-help-item">
                                <i class="fas fa-info-circle"></i>
                                <strong>Dica:</strong> Use <code>@Id</code> na sua consulta para referenciar o ID do registro atual
                            </div>
                            <div class="query-help-item">
                                <i class="fas fa-table"></i>
                                <strong>Resultado:</strong> Os dados retornados serão exibidos em formato de tabela
                            </div>
                            <div class="query-help-item">
                                <i class="fas fa-columns"></i>
                                <strong>Colunas:</strong> As colunas da consulta serão usadas como títulos da tabela
                            </div>
                        </div>
                    </div>
                ` : `
                    <div class="section-fields-area"
                         data-section-id="${section.id}"
                         ondrop="TemplateBuilder.handleDrop(event, '${section.id}')"
                         ondragover="TemplateBuilder.handleDragOver(event)"
                         ondragleave="TemplateBuilder.handleDragLeave(event)">
                        ${section.fields.length === 0 ?
                            '<div style="text-align: center; color: #999; padding: 20px;">Arraste campos aqui</div>' :
                            this.renderFields(section.fields, section.id)
                        }
                    </div>
                `}
            `;

            canvas.appendChild(sectionDiv);
        });
    },

    /**
     * Renderizar campos de uma seção
     */
    renderFields(fields, sectionId) {
        return fields.map(field => {
            // Aceitar tanto camelCase quanto PascalCase
            const icon = field.icon || field.Icon || 'fas fa-file';
            const label = field.label || field.Label || '';
            const propertyName = field.propertyName || field.PropertyName || '';

            return `
                <div class="field-item" draggable="true">
                    <div>
                        <i class="${icon}"></i>
                        <strong>${label}</strong>
                        <small style="color: #999; margin-left: 10px;">${propertyName}</small>
                    </div>
                    <button class="section-control-btn danger" onclick="TemplateBuilder.removeField('${sectionId}', '${propertyName}')">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            `;
        }).join('');
    },

    /**
     * Handlers de drag and drop
     */
    handleDragOver(e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'copy';
        e.currentTarget.classList.add('drag-over');
    },

    handleDragLeave(e) {
        e.currentTarget.classList.remove('drag-over');
    },

    handleDrop(e, sectionId) {
        e.preventDefault();
        e.currentTarget.classList.remove('drag-over');

        try {
            const data = JSON.parse(e.dataTransfer.getData('application/json'));
            this.addFieldToSection(sectionId, data);
        } catch (error) {
            console.error('Erro ao processar drop:', error);
        }
    },

    /**
     * Adicionar campo a uma seção
     */
    addFieldToSection(sectionId, property) {
        const section = this.sections.find(s => s.id === sectionId);
        if (!section) return;

        // Verificar se o campo já existe (case-insensitive)
        if (section.fields.some(f => {
            const existingProp = f.propertyName || f.PropertyName;
            return existingProp === property.propertyName;
        })) {
            this.showInfo('Campo já adicionado a esta seção');
            return;
        }

        section.fields.push({
            label: property.label,
            propertyName: property.propertyName,
            format: this.getDefaultFormat(property.propertyType),
            order: section.fields.length,
            icon: property.icon,
            displayType: 'default',
            bold: false
        });

        this.renderSections();
    },

    /**
     * Obter formato padrão baseado no tipo
     */
    getDefaultFormat(type) {
        switch (type) {
            case 'date': return 'dd/MM/yyyy';
            case 'currency': return 'C2';
            default: return null;
        }
    },

    /**
     * Atualizar título da seção
     */
    updateSectionTitle(sectionId, title) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            section.title = title;
        }
    },

    /**
     * Mudar tipo de seção
     */
    changeSectionType(sectionId, type) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            section.type = type;
            this.renderSections();
        }
    },

    /**
     * Mudar número de colunas
     */
    changeSectionColumns(sectionId, columns) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            section.columns = parseInt(columns);
        }
    },

    /**
     * Mover seção
     */
    moveSection(sectionId, direction) {
        const index = this.sections.findIndex(s => s.id === sectionId);
        if (index === -1) return;

        if (direction === 'up' && index > 0) {
            [this.sections[index], this.sections[index - 1]] = [this.sections[index - 1], this.sections[index]];
        } else if (direction === 'down' && index < this.sections.length - 1) {
            [this.sections[index], this.sections[index + 1]] = [this.sections[index + 1], this.sections[index]];
        }

        this.sections.forEach((s, i) => s.order = i);
        this.renderSections();
    },

    /**
     * Remover seção
     */
    removeSection(sectionId) {
        if (!confirm('Deseja remover esta seção?')) return;

        this.sections = this.sections.filter(s => s.id !== sectionId);
        this.renderSections();
    },

    /**
     * Remover campo
     */
    removeField(sectionId, propertyName) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            section.fields = section.fields.filter(f => {
                const fieldProp = f.propertyName || f.PropertyName;
                return fieldProp !== propertyName;
            });
            this.renderSections();
        }
    },

    /**
     * Gerar JSON do template
     */
    buildTemplate() {
        const entityType = document.getElementById('entitySelector').value;

        if (!entityType) {
            throw new Error('Selecione uma entidade');
        }

        if (this.sections.length === 0) {
            throw new Error('Adicione pelo menos uma seção ao relatório');
        }

        // Salvar conteúdo dos editores de texto rico e queries antes de gerar o template
        this.sections.forEach(section => {
            if (section.type === 'richtext') {
                const editor = document.getElementById(`richtext-${section.id}`);
                if (editor) {
                    section.richTextContent = editor.innerHTML;
                }
            } else if (section.type === 'external_query') {
                const queryEditor = document.getElementById(`query-${section.id}`);
                if (queryEditor) {
                    section.sqlQuery = queryEditor.value;
                }
            }
        });

        return {
            name: document.getElementById('templateName').value || 'Template Sem Nome',
            sections: this.sections.map((section, index) => ({
                title: section.title,
                subtitle: '',
                type: section.type,
                columns: section.columns,
                order: index,
                fields: section.fields.map((field, i) => ({
                    label: field.label || field.Label,
                    propertyName: field.propertyName || field.PropertyName,
                    format: field.format || field.Format,
                    order: i,
                    displayType: field.displayType || field.DisplayType || 'default',
                    bold: field.bold || field.Bold || false
                })),
                listColumns: section.listColumns || [],
                dataProperty: section.dataProperty || null,
                showTotal: section.showTotal || false,
                totalField: section.totalField || null,
                icon: section.icon,
                richTextContent: section.richTextContent || '',
                sqlQuery: section.sqlQuery || ''
            }))
        };
    },

    /**
     * Salvar template
     */
    async save() {
        try {
            const template = this.buildTemplate();
            const entityType = document.getElementById('entitySelector').value;
            const name = document.getElementById('templateName').value;
            const description = document.getElementById('templateDescription').value;
            const isDefault = document.getElementById('isDefaultTemplate').checked;

            if (!name) {
                this.showError('Informe um nome para o template');
                return;
            }

            const payload = {
                templateId: this.templateId,
                name: name,
                entityType: entityType,
                description: description,
                isDefault: isDefault,
                template: template
            };

            const response = await fetch('/ReportBuilder/Save', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (result.success) {
                this.showSuccess(result.message);

                // Redirecionar após salvar
                setTimeout(() => {
                    window.location.href = '/ReportTemplate';
                }, 1500);
            } else {
                this.showError(result.message);
            }
        } catch (error) {
            console.error('Erro ao salvar:', error);
            this.showError(error.message);
        }
    },

    /**
     * Preview do template
     */
    async preview() {
        try {
            const template = this.buildTemplate();
            const entityType = document.getElementById('entitySelector').value;

            const response = await fetch('/ReportBuilder/Preview', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    entityType: entityType,
                    template: template
                })
            });

            const result = await response.json();

            if (result.success) {
                document.getElementById('previewContent').innerHTML = result.html;
                const modal = new bootstrap.Modal(document.getElementById('previewModal'));
                modal.show();
            } else {
                this.showError(result.message);
            }
        } catch (error) {
            console.error('Erro ao gerar preview:', error);
            this.showError(error.message);
        }
    },

    /**
     * Executar comando de formatação no editor de texto rico
     */
    execCommand(command) {
        document.execCommand(command, false, null);
    },

    /**
     * Mudar cor do texto
     */
    changeTextColor(color, sectionId) {
        if (!color) return;
        document.execCommand('foreColor', false, color);
        // Reset select
        const select = event.target;
        select.selectedIndex = 0;
    },

    /**
     * Mudar tamanho da fonte
     */
    changeFontSize(size, sectionId) {
        if (!size) return;
        document.execCommand('fontSize', false, size);
        // Reset select
        const select = event.target;
        select.selectedIndex = 0;
    },

    /**
     * Inserir imagem no editor
     */
    insertImage(sectionId) {
        const url = prompt('Digite a URL da imagem:');
        if (url) {
            document.execCommand('insertImage', false, url);
        }
    },

    /**
     * Salvar conteúdo do editor de texto rico
     */
    saveRichTextContent(sectionId) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            const editor = document.getElementById(`richtext-${sectionId}`);
            if (editor) {
                section.richTextContent = editor.innerHTML;
            }
        }
    },

    /**
     * Salvar conteúdo da consulta SQL
     */
    saveQueryContent(sectionId) {
        const section = this.sections.find(s => s.id === sectionId);
        if (section) {
            const queryEditor = document.getElementById(`query-${sectionId}`);
            if (queryEditor) {
                section.sqlQuery = queryEditor.value;
            }
        }
    },

    /**
     * Carregar template existente
     */
    loadTemplate(templateJson) {
        try {
            // Se templateJson é string, fazer parse. Se já é objeto, usar direto
            const template = typeof templateJson === 'string'
                ? JSON.parse(templateJson)
                : templateJson;

            console.log('Template recebido:', template);

            if (!template) {
                throw new Error('Template inválido');
            }

            // Aceitar tanto 'sections' (minúsculo) quanto 'Sections' (maiúsculo)
            const sections = template.sections || template.Sections;

            if (!sections || !Array.isArray(sections)) {
                console.error('Template sem seções válidas:', template);
                throw new Error('Template inválido ou sem seções');
            }

            this.sections = sections.map((section, index) => ({
                id: 'section-' + index,
                title: section.title || section.Title,
                type: section.type || section.Type,
                columns: section.columns || section.Columns,
                order: section.order || section.Order,
                fields: section.fields || section.Fields || [],
                icon: section.icon || section.Icon || 'fas fa-folder',
                subtitle: section.subtitle || section.Subtitle,
                dataProperty: section.dataProperty || section.DataProperty,
                listColumns: section.listColumns || section.ListColumns || [],
                showTotal: section.showTotal || section.ShowTotal,
                totalField: section.totalField || section.TotalField,
                richTextContent: section.richTextContent || section.RichTextContent || '',
                sqlQuery: section.sqlQuery || section.SqlQuery || ''
            }));

            console.log('Seções carregadas:', this.sections);
            this.renderSections();
        } catch (error) {
            console.error('Erro ao carregar template:', error);
            this.showError('Erro ao carregar template: ' + error.message);
        }
    },

    /**
     * Mostrar mensagens
     */
    showSuccess(message) {
        this.showToast(message, 'success');
    },

    showError(message) {
        this.showToast(message, 'danger');
    },

    showInfo(message) {
        this.showToast(message, 'info');
    },

    showToast(message, type) {
        // Usar sistema de toast do Bootstrap ou criar alert simples
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3"
                 style="z-index: 9999; min-width: 400px;" role="alert">
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'danger' ? 'exclamation-circle' : 'info-circle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', alertHtml);

        setTimeout(() => {
            const alert = document.querySelector('.alert');
            if (alert) {
                alert.classList.remove('show');
                setTimeout(() => alert.remove(), 150);
            }
        }, 3000);
    }
};

// Exportar globalmente
window.TemplateBuilder = TemplateBuilder;
