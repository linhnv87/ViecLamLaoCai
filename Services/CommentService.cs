using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Database.Models;
using Repositories;
using Services.DTO;

namespace Services
{
    public interface ICommentService
    {
        Task<CommentDTO> CreateComment(CommentDTO payload);
        Task<List<CommentDTO>> GetAllComments();
        Task<List<CommentDTO>> GetDocumentComments(int docId);
    }
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public CommentService(ICommentRepository commentRepository, IMapper mapper, IUserRepository userRepository)
        {
            this.commentRepository = commentRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }
        public async Task<CommentDTO> CreateComment(CommentDTO payload)
        {
            try
            {
                var data = mapper.Map<CommentDTO, TblComment>(payload);
                await commentRepository.AddAsync(data);
                await commentRepository.SaveChanges();

                return mapper.Map<TblComment, CommentDTO>(data);

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CommentDTO>> GetAllComments()
        {
            try
            {
                var data = commentRepository.GetAll();

                return mapper.Map<List<CommentDTO>>(data);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<CommentDTO>> GetDocumentComments(int docId)
        {
            try
            {
                var data = await commentRepository.GetMulti(x => x.DocId == docId);                
                var dtos = mapper.Map<List<CommentDTO>>(data);
                foreach(var item in dtos){
                    var user = await userRepository.FirstOrDefaultAsync(x => x.UserId == item.UserId);
                    if(user != null)
                    {
                        item.UserName = user.UserFullName;
                    }
                }
                return dtos;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
