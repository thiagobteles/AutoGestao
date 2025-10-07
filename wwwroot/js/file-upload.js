(function () {
    'use strict';

    class FileUploadHandler {
        constructor() {
            this.init();
        }

        init() {
            document.addEventListener('DOMContentLoaded', () => {
                this.attachFileInputListeners();
                this.attachDeleteFileListeners();
                this.attachClearFileListeners();
                this.attachLabelClickListeners();
            });

            document.addEventListener('formContentLoaded', () => {
                this.attachFileInputListeners();
                this.attachDeleteFileListeners();
                this.attachClearFileListeners();
                this.attachLabelClickListeners();
            });
        }

        attachFileInputListeners() {
            const fileInputs = document.querySelectorAll('.file-input');
            fileInputs.forEach(input => {
                if (!input.dataset.listenerAttached) {
                    input.addEventListener('change', (e) => {
                        this.updateLabelText(e);
                        this.handleFileUpload(e);
                    });
                    input.dataset.listenerAttached = 'true';
                }
            });
        }

        attachDeleteFileListeners() {
            const deleteButtons = document.querySelectorAll('.btn-delete-file');
            deleteButtons.forEach(btn => {
                if (!btn.dataset.listenerAttached) {
                    btn.addEventListener('click', (e) => this.handleFileDelete(e));
                    btn.dataset.listenerAttached = 'true';
                }
            });
        }

        attachClearFileListeners() {
            const clearButtons = document.querySelectorAll('.file-upload-clear');
            clearButtons.forEach(btn => {
                if (!btn.dataset.listenerAttached) {
                    btn.addEventListener('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.clearFileSelection(e);
                    });
                    btn.dataset.listenerAttached = 'true';
                }
            });
        }

        attachLabelClickListeners() {
            const labels = document.querySelectorAll('.file-upload-label');
            labels.forEach(label => {
                // Prevenir que o clique no botão clear acione o input
                label.addEventListener('click', (e) => {
                    if (e.target.closest('.file-upload-clear')) {
                        e.preventDefault();
                        e.stopPropagation();
                    }
                });
            });
        }

        updateLabelText(event) {
            const input = event.target;
            const label = input.nextElementSibling;

            if (!label) return;

            const textSpan = label.querySelector('.file-upload-text');
            const file = input.files[0];

            if (file) {
                textSpan.textContent = file.name;
                label.classList.add('has-file');

                // Adicionar botão clear se não existir
                if (!label.querySelector('.file-upload-clear')) {
                    const clearBtn = document.createElement('button');
                    clearBtn.type = 'button';
                    clearBtn.className = 'file-upload-clear';
                    clearBtn.title = 'Limpar seleção';
                    clearBtn.innerHTML = '<i class="fas fa-times"></i>';
                    clearBtn.addEventListener('click', (e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        this.clearFileSelection(e);
                    });
                    label.appendChild(clearBtn);
                }
            } else {
                textSpan.textContent = 'Nenhum arquivo selecionado';
                label.classList.remove('has-file');
                const clearBtn = label.querySelector('.file-upload-clear');
                if (clearBtn) clearBtn.remove();
            }
        }

        clearFileSelection(event) {
            const clearBtn = event.currentTarget;
            const label = clearBtn.closest('.file-upload-label');
            const container = label.closest('.file-upload-wrapper');
            const input = container.querySelector('.file-input');
            const textSpan = label.querySelector('.file-upload-text');
            const hiddenInput = container.parentElement.querySelector('.file-path-input');

            input.value = '';
            if (hiddenInput) hiddenInput.value = '';
            textSpan.textContent = 'Nenhum arquivo selecionado';
            label.classList.remove('has-file');
            clearBtn.remove();
        }

        async handleFileUpload(event) {
            const input = event.target;
            const file = input.files[0];

            if (!file) return;

            const propertyName = input.dataset.property;
            const customBucket = input.dataset.customBucket;
            const container = input.closest('.image-upload-container, .file-upload-container');
            const hiddenInput = document.getElementById(`hidden_${propertyName}`);

            console.log('[UPLOAD] Iniciando upload...', {
                propertyName,
                fileName: file.name,
                hiddenInput: hiddenInput?.id,
                hiddenInputValue: hiddenInput?.value
            });

            if (!hiddenInput) {
                console.error('[UPLOAD] Hidden input não encontrado!');
                return;
            }

            const formData = new FormData();
            formData.append('file', file);
            formData.append('propertyName', propertyName);
            if (customBucket) {
                formData.append('customBucket', customBucket);
            }

            try {
                this.showLoading(container);

                const controllerName = this.getControllerName();
                const response = await fetch(`/${controllerName}/UploadFile`, {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    console.log('[UPLOAD] Sucesso!', {
                        filePath: result.filePath,
                        fileUrl: result.fileUrl
                    });

                    // ATUALIZAR O HIDDEN INPUT COM O FILEPATH
                    hiddenInput.value = result.filePath;
                    console.log('[UPLOAD] Hidden input atualizado:', {
                        id: hiddenInput.id,
                        name: hiddenInput.name,
                        value: hiddenInput.value
                    });

                    this.showSuccess(container, result.message);

                    // Substituir campo de upload por preview
                    this.replaceUploadWithPreview(container, result, input.closest('.form-group-modern'), propertyName);
                } else {
                    this.showError(container, result.message);
                    input.value = '';
                    this.resetLabel(input);
                }
            } catch (error) {
                this.showError(container, 'Erro ao enviar arquivo');
                console.error('[UPLOAD] Erro:', error);
                input.value = '';
                this.resetLabel(input);
            } finally {
                this.hideLoading(container);
            }
        }


        replaceUploadWithPreview(container, result, formGroup, propertyName) {
            // Remover o campo de upload
            const uploadWrapper = container.querySelector('.file-upload-wrapper');
            if (uploadWrapper) {
                uploadWrapper.remove();
            }

            const isImage = container.classList.contains('image-upload-container');

            if (isImage) {
                // Criar preview de imagem
                const previewHtml = `
            <div class="image-preview-wrapper">
                <img src="${result.fileUrl}" 
                     alt="Imagem" 
                     class="image-preview" 
                     style="max-width: 200px; max-height: 200px; object-fit: cover; border-radius: 8px;">
                <button type="button" 
                        class="btn btn-sm btn-danger btn-delete-file" 
                        data-property="${propertyName}" 
                        data-filepath="${result.filePath}"
                        title="Excluir arquivo">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;
                container.insertAdjacentHTML('afterbegin', previewHtml);
            } else {
                // Criar preview de arquivo
                const previewHtml = `
            <div class="file-info">
                <i class="fas fa-file me-2"></i>
                <span class="file-name">${result.fileName}</span>
                <a href="${result.fileUrl}" target="_blank" class="btn btn-sm btn-outline-primary ms-2" title="Visualizar">
                    <i class="fas fa-eye"></i>
                </a>
                <button type="button" 
                        class="btn btn-sm btn-danger btn-delete-file" 
                        data-property="${propertyName}" 
                        data-filepath="${result.filePath}"
                        title="Excluir arquivo">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;
                container.insertAdjacentHTML('afterbegin', previewHtml);
            }

            // Reattach listeners para o botão de deletar
            this.attachDeleteFileListeners();
        }

        resetLabel(input) {
            const label = input.nextElementSibling;
            if (label) {
                const textSpan = label.querySelector('.file-upload-text');
                textSpan.textContent = 'Nenhum arquivo selecionado';
                label.classList.remove('has-file');
                const clearBtn = label.querySelector('.file-upload-clear');
                if (clearBtn) clearBtn.remove();
            }
        }

        async handleFileDelete(event) {
            event.preventDefault();
            event.stopPropagation();

            if (!confirm('Deseja realmente excluir este arquivo?')) {
                return;
            }

            const btn = event.currentTarget;
            const propertyName = btn.dataset.property;
            const filePath = btn.dataset.filepath;
            const container = btn.closest('.image-upload-container, .file-upload-container');
            const hiddenInput = document.getElementById(`hidden_${propertyName}`);

            console.log('[DELETE] Iniciando exclusão...', {
                propertyName,
                filePath,
                hiddenInput: hiddenInput?.id
            });

            if (!hiddenInput) {
                console.error('[DELETE] Hidden input não encontrado!');
                return;
            }

            try {
                this.showLoading(container);

                const controllerName = this.getControllerName();
                const response = await fetch(`/${controllerName}/DeleteFile`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        propertyName: propertyName,
                        filePath: filePath
                    })
                });

                const result = await response.json();

                if (result.success) {
                    console.log('[DELETE] Sucesso!');

                    // LIMPAR O HIDDEN INPUT
                    hiddenInput.value = '';
                    console.log('[DELETE] Hidden input limpo:', {
                        id: hiddenInput.id,
                        name: hiddenInput.name,
                        value: hiddenInput.value
                    });

                    // Remover preview e recriar campo de upload
                    this.recreateUploadField(container, propertyName);
                    this.showSuccess(container, result.message);
                } else {
                    this.showError(container, result.message);
                }
            } catch (error) {
                this.showError(container, 'Erro ao excluir arquivo');
                console.error('[DELETE] Erro:', error);
            } finally {
                this.hideLoading(container);
            }
        }

        recreateUploadField(container, propertyName) {
            // Remover preview/fileInfo
            const preview = container.querySelector('.image-preview-wrapper');
            if (preview) preview.remove();

            const fileInfo = container.querySelector('.file-info');
            if (fileInfo) fileInfo.remove();

            // Determinar tipo
            const isImage = container.classList.contains('image-upload-container');
            const accept = isImage ? 'image/*' : '';

            // Criar novo campo de upload
            const uploadHtml = `
        <div class="file-upload-wrapper">
            <input type="file" 
                   class="form-control file-input" 
                   id="file_${propertyName}" 
                   data-property="${propertyName}"
                   ${accept ? `accept="${accept}"` : ''} />
            
            <label for="file_${propertyName}" class="file-upload-label">
                <span class="file-upload-btn">
                    <i class="fas fa-upload file-upload-icon"></i> Escolher Arquivo
                </span>
                <span class="file-upload-text">Nenhum arquivo selecionado</span>
            </label>
        </div>
    `;

            container.insertAdjacentHTML('afterbegin', uploadHtml);

            // Reattach listeners
            this.attachFileInputListeners();
            this.attachLabelClickListeners();
        }

        updatePreview(container, result, formGroup) {
            if (container.classList.contains('image-upload')) {
                const preview = container.querySelector('.image-preview-wrapper') || this.createImagePreviewWrapper(container);
                const img = preview.querySelector('.image-preview');
                img.src = result.fileUrl;

                if (!preview.querySelector('.btn-delete-file')) {
                    const deleteBtn = this.createDeleteButton(result.filePath, result.filePath.split('/')[0]);
                    preview.appendChild(deleteBtn);
                    this.attachDeleteFileListeners();
                }
            } else if (container.classList.contains('file-upload')) {
                const fileInfo = container.querySelector('.file-info') || this.createFileInfoDiv(container);
                fileInfo.querySelector('.file-name').textContent = result.fileName;

                if (!fileInfo.querySelector('.btn-delete-file')) {
                    const deleteBtn = this.createDeleteButton(result.filePath, result.filePath.split('/')[0]);
                    fileInfo.appendChild(deleteBtn);
                    this.attachDeleteFileListeners();
                }
            }
        }

        createImagePreviewWrapper(container) {
            const wrapper = document.createElement('div');
            wrapper.className = 'image-preview-wrapper';
            wrapper.innerHTML = '<img class="image-preview" style="max-width: 100px; max-height: 100px; border-radius: 4px;">';
            container.insertBefore(wrapper, container.querySelector('.file-upload-wrapper'));
            return wrapper;
        }

        createFileInfoDiv(container) {
            const div = document.createElement('div');
            div.className = 'file-info';
            div.innerHTML = '<i class="fas fa-file file-upload-icon text-primary"></i><span class="file-name"></span>';
            container.insertBefore(div, container.querySelector('.file-upload-wrapper'));
            return div;
        }

        createDeleteButton(filePath, propertyName) {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn btn-sm btn-danger btn-delete-file';
            btn.dataset.property = propertyName;
            btn.dataset.filepath = filePath; // USAR filepath ao invés de filename
            btn.title = 'Excluir arquivo';
            btn.innerHTML = '<i class="fas fa-trash"></i>';
            return btn;
        }

        clearPreview(container) {
            const preview = container.querySelector('.image-preview-wrapper');
            if (preview) preview.remove();

            const fileInfo = container.querySelector('.file-info');
            if (fileInfo) fileInfo.remove();
        }

        showLoading(container) {
            const spinner = document.createElement('div');
            spinner.className = 'spinner-border spinner-border-sm text-primary upload-spinner ms-2';
            spinner.setAttribute('role', 'status');
            const wrapper = container.querySelector('.file-upload-wrapper');
            if (wrapper) wrapper.appendChild(spinner);
        }

        hideLoading(container) {
            const spinner = container.querySelector('.upload-spinner');
            if (spinner) spinner.remove();
        }

        showSuccess(container, message) {
            this.showToast(message, 'success');
        }

        showError(container, message) {
            this.showToast(message, 'danger');
        }

        showToast(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 end-0 m-3`;
            toast.style.zIndex = '9999';
            toast.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            document.body.appendChild(toast);

            setTimeout(() => {
                toast.classList.remove('show');
                setTimeout(() => toast.remove(), 150);
            }, 3000);
        }

        getControllerName() {
            const path = window.location.pathname.split('/');
            return path[1] || '';
        }
    }

    new FileUploadHandler();
})();