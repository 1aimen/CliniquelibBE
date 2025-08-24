namespace Cliniquelib_BE.Models.Enums
{

    public enum RoleEnum
    {
        Admin, // admin of an org
        Doctor,
        Nurse,
        Reception,
        Staff,
        Patient,
        SuperAdmin // admin entity that lives out of orgs has the possibility to create many org's if needed
    }


}

