using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.DataAccess;
using DatingApp.API.Dtos;
using DatingApp.API.Logic;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _map;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;
        public PhotosController(IDatingRepository datingRepository, IMapper map, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _datingRepository = datingRepository;
            _map = map;
            _cloudinaryConfig = cloudinaryConfig;

            // creating our cloudinary connection for storeing photos
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _datingRepository.getPhoto(id);

            var photoToReturn = _map.Map<PhotoForReturnDto>(photo);

            return Ok(photoToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, 
            [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            // if the user is not same user as the token passed in
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userRepo = await _datingRepository.GetUser(userId);

            var file = photoForCreationDto.File;

            var result = new ImageUploadResult();

            // check to see if there is anything inside the file
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    result = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = result.Uri.ToString();
            photoForCreationDto.PublicId = result.PublicId;

            var photo = _map.Map<Photo>(photoForCreationDto);

            // if first photo uploaded, set as main photo for the user
            if (!userRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userRepo.Photos.Add(photo);

            if (await _datingRepository.SaveAll())
            {
                var photoToReturn = _map.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            // if the user is not same user as the token passed in
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _datingRepository.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.getPhoto(id);

            // checks to see if the photo is allready the main
            if (photoFromRepo.IsMain)
            {
                return BadRequest("This photo is already the main photo");
            }

            var currentMainPhoto = await _datingRepository.getMainPhotoForUser(userId);

            // here we change the main photo
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            // save changes
            if (await _datingRepository.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // if the user is not same user as the token passed in
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _datingRepository.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await _datingRepository.getPhoto(id);

            // checks to see if the photo is allready the main
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Can not delete main photo");
            }

            // checks to see fi the photo has a public id for cloudinary
            // if it is it will delete from cloudinary if not it will just deleted from db
            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _datingRepository.delete(photoFromRepo);
                }
            }
            else 
            {
                _datingRepository.delete(photoFromRepo);
            }

            if (await _datingRepository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
    }
}