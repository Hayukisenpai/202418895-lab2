using System;
using System.Linq;

public class Employee
{
    private string id;
    private string fullName;
    private double baseSalary;

    public Employee() : this("UNKNOWN", "Unnamed employee", 0.0) { }

    public Employee(string id, string fullName) : this(id, fullName, 0.0) { }

    public Employee(string id, string fullName, double baseSalary)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Mã nhân viên không được rỗng.", nameof(id));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không được rỗng.", nameof(fullName));
        if (baseSalary < 0)
            throw new ArgumentException("Lương cơ bản không được âm.", nameof(baseSalary));

        this.id = id;
        this.fullName = fullName;
        this.baseSalary = baseSalary;

        Console.WriteLine($"[Employee] Đã tạo: {this.id} - {this.fullName}");
    }

    public string Id => id;
    public string FullName => fullName;
    public double BaseSalary => baseSalary;

    public void IncreaseSalary(double amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Giá trị tăng phải dương.", nameof(amount));
        baseSalary += amount;
    }

    public void IncreaseSalary(double value, bool byPercentage)
    {
        if (value <= 0)
            throw new ArgumentException("Giá trị tăng phải dương.", nameof(value));

        if (byPercentage)
            baseSalary += baseSalary * value / 100.0;
        else
            baseSalary += value;
    }

    public virtual double CalculateMonthlyCost()
    {
        return baseSalary;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"ID: {id} | Họ tên: {fullName} | Lương cơ bản: {baseSalary:N0} | Chi phí tháng: {CalculateMonthlyCost():N0}");
    }

    ~Employee()
    {
        Console.WriteLine($"[Employee] Đã hủy: {id} - {fullName}");
    }
}

public class SoftwareEngineer : Employee
{
    private string primaryLanguage;
    private double technicalAllowance;

    // Constructor 1: không có lương và phụ cấp (mặc định = 0)
    public SoftwareEngineer(string id, string fullName, string primaryLanguage)
        : this(id, fullName, 0.0, primaryLanguage, 0.0) { }

    // Constructor 2: đầy đủ tham số
    public SoftwareEngineer(string id, string fullName, double baseSalary,
                            string primaryLanguage, double technicalAllowance)
        : base(id, fullName, baseSalary)
    {
        if (string.IsNullOrWhiteSpace(primaryLanguage))
            throw new ArgumentException("Ngôn ngữ chính không được rỗng.", nameof(primaryLanguage));
        if (technicalAllowance < 0)
            throw new ArgumentException("Phụ cấp không được âm.", nameof(technicalAllowance));

        this.primaryLanguage = primaryLanguage;
        this.technicalAllowance = technicalAllowance;

        Console.WriteLine($"[SoftwareEngineer] Đã tạo: {Id} - {primaryLanguage}");
    }

    // Getter
    public string PrimaryLanguage => primaryLanguage;
    public double TechnicalAllowance => technicalAllowance;

    // Ghi đè: tổng lương cơ bản và phụ cấp
    public override double CalculateMonthlyCost()
    {
        return base.CalculateMonthlyCost() + technicalAllowance;
    }

    // Ghi đè: hiển thị thêm ngôn ngữ và phụ cấp
    public override void DisplayInfo()
    {
        Console.WriteLine($"[SoftwareEngineer] ID: {Id} | Họ tên: {FullName} | Ngôn ngữ: {primaryLanguage} | " +
                          $"Lương cơ bản: {BaseSalary:N0} | Phụ cấp: {technicalAllowance:N0} | " +
                          $"Chi phí tháng: {CalculateMonthlyCost():N0}");
    }

    // Finalizer
    ~SoftwareEngineer()
    {
        Console.WriteLine($"[SoftwareEngineer] Đã hủy: {Id} - {primaryLanguage}");
    }
}

public class ProjectTeam
{
    private string projectCode;
    private string projectName;
    private Employee leader;                          // liên kết không sở hữu, có thể null
    private List<Employee> members = new List<Employee>();  // liên kết không sở hữu

    // ===== Constructor =====

    // Nhóm chưa có trưởng nhóm
    public ProjectTeam(string projectCode, string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectCode))
            throw new ArgumentException("Mã dự án không được rỗng.", nameof(projectCode));
        if (string.IsNullOrWhiteSpace(projectName))
            throw new ArgumentException("Tên dự án không được rỗng.", nameof(projectName));

        this.projectCode = projectCode;
        this.projectName = projectName;
        this.leader = null;

        Console.WriteLine($"[ProjectTeam] Đã tạo: {projectCode} - {projectName}");
    }

    // Thiết lập trưởng nhóm và tự động đưa vào danh sách thành viên
    public ProjectTeam(string projectCode, string projectName, Employee leader)
        : this(projectCode, projectName)
    {
        if (leader == null)
            throw new ArgumentNullException(nameof(leader), "Trưởng nhóm không được null.");

        members.Add(leader);
        this.leader = leader;
    }

    // ===== Getter =====
    public string ProjectCode => projectCode;
    public string ProjectName => projectName;
    public Employee Leader => leader;
    public int MemberCount => members.Count;

    // ===== Nạp chồng addMember =====

    // Thêm thành viên (không thêm trùng)
    public bool AddMember(Employee employee)
    {
        if (employee == null || Contains(employee.Id))
            return false;

        members.Add(employee);
        return true;
    }

    // Thêm thành viên, nếu makeLeader == true thì trở thành trưởng nhóm
    public bool AddMember(Employee employee, bool makeLeader)
    {
        if (!AddMember(employee))
            return false;

        if (makeLeader)
            leader = employee;   // trưởng nhóm cũ vẫn nằm trong danh sách thành viên

        return true;
    }

    // ===== Các phương thức khác =====

    // Xóa thành viên theo mã. Không được xóa trưởng nhóm hiện tại.
    public bool RemoveMember(string employeeId)
    {
        if (leader != null && leader.Id == employeeId)
            return false;   // phải đổi trưởng nhóm (ChangeLeader) trước

        Employee found = members.FirstOrDefault(m => m.Id == employeeId);
        if (found == null)
            return false;

        members.Remove(found);
        return true;
    }

    // Đổi trưởng nhóm. Nếu người mới chưa là thành viên thì thêm vào nhóm.
    public bool ChangeLeader(Employee employee)
    {
        if (employee == null)
            return false;

        if (!Contains(employee.Id))
            members.Add(employee);

        // Lấy đúng đối tượng đang nằm trong danh sách
        leader = members.First(m => m.Id == employee.Id);
        return true;
    }

    // Kiểm tra có thành viên với mã này không
    public bool Contains(string employeeId)
    {
        return members.Any(m => m.Id == employeeId);
    }

    // Tổng chi phí hàng tháng của cả nhóm (gọi bản override qua đa hình)
    public double CalculateTotalMonthlyCost()
    {
        return members.Sum(m => m.CalculateMonthlyCost());
    }

    // Hiển thị thông tin nhóm
    public void DisplayTeam()
    {
        Console.WriteLine("===== PROJECT TEAM =====");
        Console.WriteLine($"Mã dự án : {projectCode}");
        Console.WriteLine($"Tên dự án: {projectName}");
        Console.WriteLine($"Trưởng nhóm: {(leader != null ? leader.FullName + " (" + leader.Id + ")" : "Chưa có")}");
        Console.WriteLine($"Số thành viên: {members.Count}");

        foreach (Employee m in members)
            m.DisplayInfo();   // virtual: Employee hay SoftwareEngineer đều in đúng bản của mình

        Console.WriteLine($"Tổng chi phí tháng: {CalculateTotalMonthlyCost():N0}");
    }

    // Finalizer: chỉ dọn danh sách liên kết, KHÔNG hủy các Employee
    ~ProjectTeam()
    {
        members.Clear();
        leader = null;
        Console.WriteLine($"[ProjectTeam] Đã hủy: {projectCode}");
    }
}
public class Program
{
    public static void Main()
    {
        Console.WriteLine("===== 1. Tạo hai Employee bằng hai constructor khác nhau =====");
        Employee e1 = new Employee("E01", "Nguyen Van A");                 // 2 tham số
        Employee e2 = new Employee("E02", "Tran Thi B", 10000000);         // 3 tham số

        Console.WriteLine("\n===== 2. Tạo hai SoftwareEngineer bằng hai constructor khác nhau =====");
        SoftwareEngineer se1 = new SoftwareEngineer("SE01", "Le Van C", "C#");   // 3 tham số
        SoftwareEngineer se2 = new SoftwareEngineer("SE02", "Pham Thi D", 18000000, "Java", 3000000); // 5 tham số

        Console.WriteLine("\n===== 3. Tăng lương một nhân sự bằng số tiền cố định =====");
        e2.IncreaseSalary(500000);
        Console.WriteLine($"Lương của {e2.FullName}: {e2.BaseSalary:N0}");   // 10.500.000

        Console.WriteLine("\n===== 4. Tăng lương một nhân sự khác theo phần trăm =====");
        se2.IncreaseSalary(10, true);
        Console.WriteLine($"Lương của {se2.FullName}: {se2.BaseSalary:N0}");  // 19.800.000

        Console.WriteLine("\n===== 5. Tạo nhóm dự án không có trưởng nhóm =====");
        ProjectTeam team1 = new ProjectTeam("P01", "He thong quan ly");
        team1.DisplayTeam();

        Console.WriteLine("\n===== 6. Thêm một nhân sự bằng AddMember(employee) =====");
        Console.WriteLine("Kết quả: " + team1.AddMember(e1));               // True

        Console.WriteLine("\n===== 7. Thêm kỹ sư bằng AddMember(employee, true) để làm trưởng nhóm =====");
        Console.WriteLine("Kết quả: " + team1.AddMember(se1, true));        // True
        Console.WriteLine($"Trưởng nhóm: {team1.Leader.FullName}");

        Console.WriteLine("\n===== 8. Thử thêm lại một thành viên đã tồn tại =====");
        Console.WriteLine("Kết quả: " + team1.AddMember(e1));               // False

        Console.WriteLine("\n===== 9. Hiển thị danh sách bằng lời gọi đa hình =====");
        team1.AddMember(se2);
        team1.DisplayTeam();

        Console.WriteLine("\n===== 10. Tính tổng chi phí nhân sự hằng tháng =====");
        Console.WriteLine($"Tổng chi phí: {team1.CalculateTotalMonthlyCost():N0}");

        Console.WriteLine("\n===== 11. Thử xóa trưởng nhóm hiện tại và kiểm tra bị từ chối =====");
        bool removed = team1.RemoveMember(se1.Id);
        Console.WriteLine("Xóa trưởng nhóm: " + removed + (removed ? "" : " (bị từ chối)"));   // False

        Console.WriteLine("\n===== 12. Đổi trưởng nhóm rồi xóa người từng là trưởng nhóm =====");
        team1.ChangeLeader(se2);
        Console.WriteLine($"Trưởng nhóm mới: {team1.Leader.FullName}");
        Console.WriteLine("Xóa trưởng nhóm cũ: " + team1.RemoveMember(se1.Id));   // True
        team1.DisplayTeam();

        Console.WriteLine("\n===== 13. Tạo nhóm thứ hai, thêm nhân sự đã có ở nhóm thứ nhất =====");
        ProjectTeam team2 = new ProjectTeam("P02", "Ung dung di dong");
        team2.AddMember(e2);
        team2.AddMember(se2);   // se2 đang ở cả team1 và team2 (quan hệ kết tập nhiều nhóm)
        Console.WriteLine($"se2 có trong nhóm 1: {team1.Contains(se2.Id)}, nhóm 2: {team2.Contains(se2.Id)}");

        Console.WriteLine("\n===== 14. Hủy nhóm thứ hai bằng cách kết thúc một khối lệnh cục bộ =====");
        {
            ProjectTeam tempTeam = new ProjectTeam("P03", "Nhom tam");
            tempTeam.AddMember(e1);
            tempTeam.AddMember(se2);
            Console.WriteLine("Nhóm tạm đang tồn tại trong khối lệnh.");
        }   // tempTeam ra khỏi phạm vi, đủ điều kiện để GC thu hồi
        team2 = null;   // nhóm thứ hai cũng bị bỏ tham chiếu
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine("\n===== 15. Chứng minh nhân sự của nhóm thứ hai vẫn tồn tại =====");
        e2.DisplayInfo();
        se2.DisplayInfo();
        Console.WriteLine("Nhân sự vẫn còn nguyên sau khi nhóm bị hủy.");

        Console.WriteLine("\n===== Kết thúc =====");
    }
}