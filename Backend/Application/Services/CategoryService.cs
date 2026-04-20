using Application.DTOs.CategoryDTOs;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using FluentValidation;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável por realizar as operações e validações envolvendo a Categoria
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CategoryDTO> _validator;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IValidator<CategoryDTO> validator)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task CreateAsync(CategoryDTO category)
        {
            await _validator.ValidateAndThrowAsync(category);
            var newCategory = _mapper.Map<Category>(category);
            await _categoryRepository.CreateAsync(newCategory);
        }

        public async Task<IList<CategoryListDTO>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IList<CategoryListDTO>>(categories);
        }

        public async Task<CategoryDTO> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                throw new Exception("Categoria não encontrada");
            }
            return _mapper.Map<CategoryDTO>(category);
        }

        public Task UpdateAsync(Guid id, CategoryDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}