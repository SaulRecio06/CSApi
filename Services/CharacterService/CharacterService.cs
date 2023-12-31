using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS.Services.CharacterService
{
    public class CharacterService : INterCharacter
    {
         private static List<Character> characters = new List<Character>{
            new Character(),
            new Character{ Id = 1, Name = "Sam"}
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character =  _mapper.Map<Character>(newCharacter);

            // character.Id = characters.Max(c => c.Id)+1; AutoIncrement antes de realizar la base de datos
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;


        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
               var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
               var dbCharacter  = await _context.Characters.FindAsync(id) ?? throw new Exception($"Character with Id '{id}' not found.");
                // var character = characters.FirstOrDefaultAsyn(c => c.Id == id) ?? throw new Exception($"Character with Id '{id}' not found.");
                _context.Characters.Remove(dbCharacter);
                await _context.SaveChangesAsync();

                serviceResponse.Data =  _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception ex) 
            {
                
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
           

            return serviceResponse;

        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;

        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
                var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character is null)
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                }

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);

                 _context.Characters.Update(character);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
           

            return serviceResponse;


        }

     
    }
}