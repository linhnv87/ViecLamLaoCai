using AutoMapper;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IDocumentHistoryService
    {
        Task<DocumentHistoryDTO> Create(DocumentHistoryDTO payload);
        Task<IEnumerable<DocumentHistoryDTO>> GetAll();
        Task<List<DocumentHistoryDTO>> GetDocumentHistory(int docId);
        Task<List<DocumentHistoryDTO>> GetDocumentHistoryByUser(string userId);
        Task<DocumentHistoryDTO> GetById(int id);
        //Task<DocumentHistoryDTO> Update(DocumentHistoryDTO payload);
        Task<int> Delete(int id);

    }

    public class DocumentHistoryService : IDocumentHistoryService
    {
        private readonly IDocumentHistoryRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public DocumentHistoryService(IDocumentHistoryRepository repository, IMapper mapper, IUserRepository userRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<DocumentHistoryDTO> Create(DocumentHistoryDTO payload)
        {
           
            var data = _mapper.Map<TblDocumentHistory>(payload);
            try
            {
                await _repository.AddAsync(data);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return _mapper.Map<DocumentHistoryDTO>(data);
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
                _repository.Remove(foundItem);
                //foundItem.Deleted = true;                
                //_repository.Update(foundItem);
                await _repository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public async Task<IEnumerable<DocumentHistoryDTO>> GetAll()
        {
            var data = _repository.GetAll().OrderByDescending(x => x.Id);
            return _mapper.Map<IEnumerable<DocumentHistoryDTO>>(data);
        }

        public async Task<DocumentHistoryDTO> GetById(int id)
        {
            var data = await _repository.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Item not found");
            }
            return _mapper.Map<DocumentHistoryDTO>(data);
        }
        public async Task<List<DocumentHistoryDTO>> GetDocumentHistory(int docId)
        {
            var historyQuery = from docHistory in _repository.GetAll()
                               join appUser in _userRepository.GetAll()
                               on docHistory.CreatedBy equals appUser.UserId into userGroup
                               from user in userGroup.DefaultIfEmpty()
                               where docHistory.DocumentId == docId
                               select new
                               {
                                   docHistory.Id,
                                   docHistory.DocumentId,
                                   DocumentTitle = docHistory.DocumentTitle ?? "",
                                   Note = docHistory.Note ?? "",
                                   docHistory.DocumentStatus,
                                   docHistory.Created,
                                   docHistory.CreatedBy,
                                   Comment = docHistory.Comment ?? "",
                                   docHistory.SubmitCount,
                                   NameUser = user != null ? user.UserFullName : "Hệ thống"
                               };
            var data = await historyQuery.OrderByDescending(x => x.Created).ToListAsync();
            return data.Select(x => new DocumentHistoryDTO
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                DocumentTitle = x.DocumentTitle,
                Note = x.Note,
                DocumentStatus = (int)x.DocumentStatus,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Comment = x.Comment,
                SubmitCount = x.SubmitCount ?? 0 ,
                NameUser = x.NameUser
            }).ToList();
        }

        public async Task<List<DocumentHistoryDTO>> GetDocumentHistoryByUser(string userId)
        {
            var data = await _repository.GetMulti(x => x.CreatedBy == Guid.Parse(userId));
            data = data.OrderByDescending(x => x.Id).ToList();
            return _mapper.Map<List<DocumentHistoryDTO>>(data);
        }

        //public async Task<DocumentHistoryDTO> Update(DocumentHistoryDTO payload)
        //{
        //    var data = await _repository.FirstOrDefaultAsync(x => x.Id == payload.Id);
        //    if (data == null)
        //    {
        //        throw new Exception($"{payload.Id} was not found");
        //    }
        //    data.Title = payload.Title;
        //    data.Active = payload.Active;
        //    await _repository.SaveChanges();
        //    return _mapper.Map<DocumentHistoryDTO>(data);
        //}
    }
}
