using AutoMapper;
using Core.QueryModels;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
        public interface IDocumentFileService
        {
            Task<DocumentFileDTO> Create(DocumentFileDTO payload);
            Task<IEnumerable<DocumentFileDTO>> GetAll();
            Task<DocumentFileDTO> GetById(int id);
            Task<DocumentFileDTO> Update(DocumentFileDTO payload);
            Task<int> Delete(int id);
            Task<List<DocumentFileDTO>> AddRelatedDocumentAsync(RelatedDocumentFileDTO payload);

        }

        public class DocumentFileService : IDocumentFileService
        {
            private readonly IDocumentFileRepository _repository;
            private readonly IMapper _mapper;
            private readonly IDocumentService _documentService;
            public DocumentFileService(IDocumentFileRepository repository, IMapper mapper,IDocumentService documentService)
            {
                _repository = repository;
                _mapper = mapper;
               _documentService = documentService;

            }
            public async Task<DocumentFileDTO> Create(DocumentFileDTO payload)
            {

                var data = _mapper.Map<TblDocumentFile>(payload);
                try
                {
                    await _repository.AddAsync(data);
                    await _repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return _mapper.Map<DocumentFileDTO>(data);
            }

            public async Task<int> Delete(int id)
            {
                var foundItem = _repository.FirstOrDefault(x => x.Id == id);
                if (foundItem == null)
                {
                    throw new Exception("Item not found");
                }
                try
                {
                     foundItem.Deleted = true;
                     _repository.Update(foundItem);
                    await _repository.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return id;
            }

            public async Task<IEnumerable<DocumentFileDTO>> GetAll()
            {
                var data = _repository.GetAll();
                return _mapper.Map<IEnumerable<DocumentFileDTO>>(data);
            }

            public async Task<DocumentFileDTO> GetById(int id)
            {
                var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
                if (data == null)
                {
                    throw new Exception("Item not found");
                }
                return _mapper.Map<DocumentFileDTO>(data);
            }

            public async Task<DocumentFileDTO> Update(DocumentFileDTO payload)
            {
                var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
                if (data == null)
                {
                    throw new Exception($"{payload.Id} was not found");
                }
                //data.Title = payload.Title;
                //data.Active = payload.Active;
                await _repository.SaveChanges();
                return _mapper.Map<DocumentFileDTO>(data);
            }
        public async Task<List<DocumentFileDTO>> AddRelatedDocumentAsync(RelatedDocumentFileDTO payload)
        {
            if (payload.SideFiles == null || payload.SideFiles.Length == 0)
                return new List<DocumentFileDTO>();

            try
            {
                var addedFiles = new List<DocumentFileDTO>();

                foreach (var file in payload.SideFiles)
                {
                    if (file.Length <= 0) continue;

                    var filePathResult = await _documentService.UploadFile(file);

                    var documentFile = new TblDocumentFile
                    {
                        FileName = file.FileName,
                        FilePath = filePathResult.Item1,
                        FilePathToView = filePathResult.Item2,
                        DocId = payload.DocId,
                        UserId = payload.ModifiedBy,
                        Modified = DateTime.Now,
                        Created = DateTime.Now,
                        Version = 1,
                        Deleted = false,
                        CreatedBy = payload.ModifiedBy,
                        ModifiedBy = payload.ModifiedBy,
                        IsFinal = false,
                        FileType = 0
                    };

                    await _repository.AddAsync(documentFile);
                    await _repository.SaveChanges();
                    addedFiles.Add(new DocumentFileDTO
                    {
                        Id = documentFile.Id,
                        FileName = documentFile.FileName,
                        FilePath = documentFile.FilePath,
                        FilePathToView = documentFile.FilePathToView,
                        Created = documentFile.Created,
                        Modified = documentFile.Modified,
                        FileType = documentFile.FileType
                    });
                }

                return addedFiles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}