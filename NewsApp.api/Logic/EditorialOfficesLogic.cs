using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Models;

namespace NewsApp.api.Logic;

public class EditorialOfficesLogic : IEditorialOfficesLogic
{
    private readonly IEditorialOfficesRepository _editorialOffices;
    private readonly IApplicationUserLogic _applicationUserLogic;

    public EditorialOfficesLogic(IEditorialOfficesRepository editorialOfficesCollection,
        IApplicationUserLogic applicationUserLogic)
    {
        _editorialOffices = editorialOfficesCollection;
        _applicationUserLogic = applicationUserLogic;
    }

    public async Task<OfficeInfo?> GetByName(string editorialOfficeName)
    {
        var result = await _editorialOffices.GetByName(editorialOfficeName);
        if (result is null)
        {
            return null;
        }

        var ownerInfo = await _applicationUserLogic.GetUserIdentity(result.OwnerId!);

        return new()
        {
            Name = result.Name,
            OwnerInfo = ownerInfo,
            Town = result.Town
        };
    }

    public async Task<OfficeInfo?> GetById(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return null;
        }

        var result = await _editorialOffices.GetById(guid);
        if (result is null)
        {
            return null;
        }

        var ownerInfo = await _applicationUserLogic.GetUserIdentity(result.OwnerId!);

        return new()
        {
            Name = result.Name,
            OwnerInfo = ownerInfo,
            Town = result.Town
        };
    }

    public async Task<BaseTypeResult<EditorialOfficeDto>> Create(NewOffice office)
    {
        try
        {
            if (await _applicationUserLogic.FindUser(office.OwnerId) is null)
            {
                return new() { Success = false, Message = "User not found" };
            }

            if (await HasEditorialOffice(office.OwnerId))
            {
                return new() { Success = false, Message = "User already has an editorial office" };
            }

            var officeDto = new EditorialOfficeDto
            {
                Name = office.Name,
                OwnerId = office.OwnerId,
                Town = office.Town
            };

            _editorialOffices.Create(officeDto);

            var result =
                await _applicationUserLogic.FindAndUpdate(officeDto.OwnerId ?? throw new InvalidOperationException(),
                    new() { EditorialOfficeId = officeDto.Id.ToString() });

            if (!result.Succeeded)
            {
                _editorialOffices.DeleteById(officeDto.Id);
                return new() { Success = false, Message = "Error while updating user info" };
            }

            return new() { Success = true, Data = officeDto };
        }
        catch (Exception ex)
        {
            return new() { Success = false, Message = ex.Message };
        }
    }

    public async Task<BaseResult> Delete(string userId, string officeId)
    {
        var userUpdated = await _applicationUserLogic.DeleteEditorialOffice(userId);

        if (!userUpdated.Succeeded)
        {
            return new() { Success = false, Message = "Error while updating user info" };
        }

        if (!Guid.TryParse(officeId, out var guid))
        {
            return new() { Success = false, Message = "Invalid id" };
        }

        try
        {
            _editorialOffices.DeleteById(guid);
        }
        catch (Exception)
        {
            return new() { Success = false, Message = "Something went wrong" };
        }

        return new() { Success = true };
    }

    public async Task<bool> IsExists(string editorialOfficeName) =>
        await _editorialOffices.GetByName(editorialOfficeName) != null;

    public async Task<bool> HasEditorialOffice(string userId) =>
        await _applicationUserLogic.HasEditorialOffice(userId);
}