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
            `;

            canvas.appendChild(sectionDiv);
        });
    },

    /**
     * Renderizar campos de uma seção
     */
    renderFields(fields, sectionId) {
        return fields.map(field => `
            <div class="field-item" draggable="true">
                <div>
                    <i class="${field.icon}"></i>
                    <strong>${field.label}</strong>
                    <small style="color: #999; margin-left: 10px;">${field.propertyName}</small>
                </div>
                <button class="section-control-btn danger" onclick="TemplateBuilder.removeField('${sectionId}', '${field.propertyName}')">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `).join('');
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

        // Verificar se o campo já existe
        if (section.fields.some(f => f.propertyName === property.propertyName)) {
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
            section.fields = section.fields.filter(f => f.propertyName !== propertyName);
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

        return {
            name: document.getElementById('templateName').value || 'Template Sem Nome',
            sections: this.sections.map((section, index) => ({
                title: section.title,
                subtitle: '',
                type: section.type,
                columns: section.columns,
                order: index,
                fields: section.fields.map((field, i) => ({
                    label: field.label,
                    propertyName: field.propertyName,
                    format: field.format,
                    order: i,
                    displayType: field.displayType || 'default',
                    bold: field.bold || false
                })),
                listColumns: [],
                dataProperty: null,
                showTotal: false,
                totalField: null,
                icon: section.icon
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
     * Carregar template existente
     */
    loadTemplate(templateJson) {
        try {
            // Se templateJson é string, fazer parse. Se já é objeto, usar direto
            const template = typeof templateJson === 'string'
                ? JSON.parse(templateJson)
                : templateJson;

            if (!template || !template.sections) {
                throw new Error('Template inválido ou sem seções');
            }

            this.sections = template.sections.map((section, index) => ({
                id: 'section-' + index,
                title: section.title,
                type: section.type,
                columns: section.columns,
                order: section.order,
                fields: section.fields,
                icon: section.icon || 'fas fa-folder'
            }));

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
