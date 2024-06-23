using Gym.Models;

namespace Gym.Interface
{
    public interface IMemberService
    {
        //duhet te marri ne parametra dhe te nxjerri VETEM VIEWMODELS
        List<MemberViewModel> GetAllMembers();
        public ServiceResult CreateMember(MemberViewModel vm);
        public MemberViewModel GetMemberByID(int id);

        public ServiceResult SoftDelete(int id);
        public List<MemberViewModel> SearchMembers(string keyword);
        public ServiceResult UpdateMember(MemberViewModel memberViewModel);
    }
}
