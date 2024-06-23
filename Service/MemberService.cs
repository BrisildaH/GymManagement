using Gym.Data;
using Gym.Data.Entities;
using Gym.Interface;
using Gym.Models;

namespace Gym.Service
{
    public class MemberService : IMemberService
    {
        private readonly ApplicationDbContext gymContext;
        public MemberService(ApplicationDbContext gymContext)
        {
            this.gymContext = gymContext;
        }

        public List<MemberViewModel> GetAllMembers()
        {
            var memberVm = new List<MemberViewModel>();
            var members = gymContext.Members.Where(p=> p.IsDeleted==false).ToList();

            foreach (var member in members)
            {
                memberVm.Add(EntityToViewModel(member));
            }
            return memberVm;
        }
        //Creating a new member 
       
        public ServiceResult CreateMember(MemberViewModel vm)
        {
            var result = new ServiceResult();

            try
            {
                var member = ViewModelToEntity(vm);

                var memberExist = gymContext.Members.Any(p => p.IdCardNumber == member.IdCardNumber);

                if (memberExist)
                {
                    result.Success = false;
                    result.Message = "The member with this ID card number already exists..";
                }
                else
                {
                    member.RegistrationDate = DateTime.Now;
                    member.IsDeleted = false;
                    gymContext.Members.Add(member);
                    gymContext.SaveChanges();

                    result.Success = true;
                    result.Message = "Member created successfully.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error creating member: " + ex.Message;
            }

            return result;
        }
        public ServiceResult SoftDelete(int id)
        {
            try
            {
                var memberExist = gymContext.Members.FirstOrDefault(m => m.ID == id);

                if (memberExist != null)
                {
                    var activeSubscription = gymContext.MemberSubscriptions.Any(ms => ms.MemberID == memberExist.ID && ms.EndDate >= DateTime.Today);
                    if (activeSubscription)
                    {
                        return new ServiceResult { Success = false, Message = "Cannot delete member. Active subscription found." };
                    }

                    memberExist.IsDeleted = true;
                    gymContext.SaveChanges();
                    return new ServiceResult { Success = true, Message = "Member deleted successfully." };
                }
                else
                {
                    return new ServiceResult { Success = false, Message = "Member not found." };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = "Error deleting member: " + ex.Message };
            }
        }

        public ServiceResult UpdateMember(MemberViewModel memberViewModel)
        {
            var result = new ServiceResult();
            try
            {
                var existingMember = gymContext.Members.FirstOrDefault(m => m.ID == memberViewModel.ID);

                if (existingMember == null)
                {
                    result.Success = false;
                    result.Message = "Member not found.";
                }
                else
                {
                    var memberWithSameIdCard = gymContext.Members.FirstOrDefault(m => m.IdCardNumber == memberViewModel.IdCardNumber && m.ID != memberViewModel.ID);

                    if (memberWithSameIdCard != null)
                    {
                        result.Success = false;
                        result.Message = "The member with this ID card number already exists.";
                    }
                    else
                    {
                        existingMember.FirstName = memberViewModel.FirstName;
                        existingMember.LastName = memberViewModel.LastName;
                        existingMember.Birthdate = memberViewModel.Birthdate;
                        existingMember.IdCardNumber = memberViewModel.IdCardNumber;
                        existingMember.Email = memberViewModel.Email;
                        existingMember.UpdateDate = DateTime.Now;

                        gymContext.SaveChanges();

                        result.Success = true;
                        result.Message = "Member updated successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error updating member: " + ex.Message;
            }

            return result;
        }

        public MemberViewModel GetMemberByID(int id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("We can't find a member, please check the Name, Last Name, Card ID or Email again");
                }

                var gymMember = gymContext.Members.FirstOrDefault(m => m.ID == id);

                if (gymMember == null)
                {
                    throw new Exception("Member not found");
                }

                return EntityToViewModel(gymMember);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching member by ID: " + id, ex);
            }
        }
        public List<MemberViewModel> SearchMembers(string keyword)
        {
            try
            {
                var members = gymContext.Members.Where(m => m.IsDeleted == false).ToList();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return members.Select(EntityToViewModel).ToList();
                }

                var filteredMembers = members.Where(m =>
                    m.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    m.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    m.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    m.IdCardNumber.Contains(keyword)
                ).ToList();

                return filteredMembers.Select(EntityToViewModel).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching members: " + ex.Message);
            }
        }

        private Member ViewModelToEntity(MemberViewModel vm)
        {
            var member = new Member()
            {
                ID = vm.ID,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Birthdate = vm.Birthdate,
                Email = vm.Email,
                IdCardNumber = vm.IdCardNumber,
                RegistrationDate = vm.RegistrationDate,
                IsDeleted = vm.IsDeleted,
                UpdateDate = vm.UpdateDate,
            };
            return member;
        }
        private MemberViewModel EntityToViewModel(Member entity)
        {
            var memberViewModel = new MemberViewModel()
            {
                ID = entity.ID,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Birthdate = entity.Birthdate,
                Email = entity.Email,
                IdCardNumber = entity.IdCardNumber,
                RegistrationDate = entity.RegistrationDate,
                IsDeleted = entity.IsDeleted,
                UpdateDate = entity.UpdateDate,
            };
            return memberViewModel;
        }
    }
}