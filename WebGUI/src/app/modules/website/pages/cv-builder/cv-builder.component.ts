import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { CandidateService } from '../../../../services/website/candidate.service';
import { SplashScreenService } from '../../../../services/splash-screen.service';
import { CVModel, WorkExperience, Education, Skill, Language, Certification, Reference } from '../../../../models/cv.model';

@Component({
  selector: 'app-cv-builder',
  templateUrl: './cv-builder.component.html',
  styleUrls: ['./cv-builder.component.scss']
})
export class CvBuilderComponent implements OnInit, OnDestroy {
  isEditMode = false;
  cvId: number | null = null;
  showExportDropdown = false;

  cvData: CVModel = {
    id: 0,
    candidateId: 1,
    title: '',
    template: 'modern',
    personalInfo: {
      fullName: '',
      email: '',
      phone: '',
      address: '',
      dateOfBirth: '',
      nationality: '',
      profileImage: '',
      linkedIn: '',
      github: '',
      portfolio: '',
      jobTitle: '',
      gender: ''
    },
    summary: '',
    experience: [this.createEmptyExperience()],
    education: [this.createEmptyEducation()],
    skills: [this.createEmptySkill()],
    languages: [this.createEmptyLanguage()],
    certifications: [this.createEmptyCertification()],
    projects: [],
    references: [this.createEmptyReference()],
    interests: '',
    computerSkills: '',
    createdDate: new Date().toISOString().split('T')[0],
    updatedDate: new Date().toISOString().split('T')[0],
    isActive: true,
    isPublic: false,
    downloadCount: 0,
    viewCount: 0
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private candidateService: CandidateService,
    private splashScreenService: SplashScreenService
  ) { }

  ngOnInit(): void {
    // Check if this is edit mode
    this.cvId = Number(this.route.snapshot.paramMap.get('id'));
    this.isEditMode = !!this.cvId;
    
    if (this.isEditMode) {
      this.loadCV();
    }
  }

  loadCV(): void {
    if (this.cvId) {
      
      const editingCV = this.candidateService.getCurrentEditingCV();
      if (editingCV && editingCV.id === this.cvId) {
        this.cvData = { ...editingCV };
        console.log('Loaded CV from edit navigation:', this.cvData.title);
        return;
      }
     
      this.candidateService.getCVsDummy(1).subscribe({
        next: (response) => {
          if (response.isSuccess && response.result) {
            const foundCV = response.result.find(cv => cv.id === this.cvId);
            if (foundCV) {
              this.cvData = { ...foundCV };
              console.log('Loaded CV from API:', this.cvData.title);
            } else {
              console.error('CV not found with ID:', this.cvId);
              this.splashScreenService.showQuickFeedback('error', 'Không tìm thấy CV!');
            }
          }
        },
        error: (error) => {
          console.error('Error loading CV:', error);
          this.splashScreenService.showQuickFeedback('error', 'Lỗi khi tải CV!');
        }
      });
    }
  }

  // Experience methods
  createEmptyExperience(): WorkExperience {
    return {
      id: Date.now(),
      jobTitle: '',
      companyName: '',
      location: '',
      startDate: '',
      endDate: '',
      isCurrentJob: false,
      description: '',
      achievements: []
    };
  }

  addExperience(): void {
    this.cvData.experience.push(this.createEmptyExperience());
  }

  removeExperience(index: number): void {
    if (this.cvData.experience.length > 1) {
      this.cvData.experience.splice(index, 1);
    }
  }

  // Education methods
  createEmptyEducation(): Education {
    return {
      id: Date.now(),
      institution: '',
      school: '',
      degree: '',
      fieldOfStudy: '',
      location: '',
      startDate: '',
      endDate: '',
      isCurrentStudy: false,
      grade: '',
      gpa: '',
      description: ''
    };
  }

  addEducation(): void {
    this.cvData.education.push(this.createEmptyEducation());
  }

  removeEducation(index: number): void {
    if (this.cvData.education.length > 1) {
      this.cvData.education.splice(index, 1);
    }
  }

  // Skills methods
  createEmptySkill(): Skill {
    return {
      id: Date.now(),
      name: '',
      level: '' as any,
      category: '' as any
    };
  }

  addSkill(): void {
    this.cvData.skills.push(this.createEmptySkill());
  }

  removeSkill(index: number): void {
    if (this.cvData.skills.length > 1) {
      this.cvData.skills.splice(index, 1);
    }
  }

  // Languages methods
  createEmptyLanguage(): Language {
    return {
      id: Date.now(),
      name: '',
      proficiency: 'Basic',
      level: '',
      certification: ''
    };
  }

  addLanguage(): void {
    this.cvData.languages.push(this.createEmptyLanguage());
  }

  removeLanguage(index: number): void {
    if (this.cvData.languages.length > 1) {
      this.cvData.languages.splice(index, 1);
    }
  }

  // Certifications methods
  createEmptyCertification(): Certification {
    return {
      id: Date.now(),
      name: '',
      organization: '',
      issuer: '',
      issueDate: '',
      expiryDate: '',
      credentialId: '',
      credentialUrl: ''
    };
  }

  addCertification(): void {
    this.cvData.certifications.push(this.createEmptyCertification());
  }

  removeCertification(index: number): void {
    if (this.cvData.certifications.length > 1) {
      this.cvData.certifications.splice(index, 1);
    }
  }

  // References methods
  createEmptyReference(): Reference {
    return {
      id: Date.now(),
      name: '',
      position: '',
      company: '',
      phone: '',
      email: '',
      relationship: ''
    };
  }

  addReference(): void {
    this.cvData.references.push(this.createEmptyReference());
  }

  removeReference(index: number): void {
    if (this.cvData.references.length > 1) {
      this.cvData.references.splice(index, 1);
    }
  }

  isFormValid(): boolean {
    // Basic validation
    const personalInfo = this.cvData.personalInfo;
    return !!(personalInfo.fullName && personalInfo.email && personalInfo.phone);
  }

  downloadCV(): void {
    if (!this.isFormValid()) {
      this.splashScreenService.showQuickFeedback('error', 'Vui lòng điền đầy đủ thông tin bắt buộc!');
      return;
    }

    // Show loading
    this.splashScreenService.showQuickFeedback('success', 'Đang tạo file PDF...');
    
    // Simulate PDF generation
    setTimeout(() => {
      this.generatePDF();
      this.splashScreenService.showQuickFeedback('success', 'Tải xuống thành công!');
    }, 2000);
  }

  generatePDF(): void {
    // In a real app, this would generate actual PDF
    // For now, we'll simulate the download
    const element = document.createElement('a');
    const filename = `CV_${this.cvData.personalInfo.fullName.replace(/\s+/g, '_')}_${new Date().toISOString().split('T')[0]}.pdf`;
    
    // Create mock PDF content
    const pdfContent = this.createPDFContent();
    const blob = new Blob([pdfContent], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    
    element.href = url;
    element.download = filename;
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
    window.URL.revokeObjectURL(url);
  }

  createPDFContent(): string {
    // This is a mock PDF content - in real app, use a PDF library like jsPDF or pdfmake
    return `
CV - ${this.cvData.personalInfo.fullName}

THÔNG TIN CÁ NHÂN
Họ và tên: ${this.cvData.personalInfo.fullName}
Chức danh: ${this.cvData.personalInfo.jobTitle}
Email: ${this.cvData.personalInfo.email}
Điện thoại: ${this.cvData.personalInfo.phone}
Địa chỉ: ${this.cvData.personalInfo.address}
Ngày sinh: ${this.cvData.personalInfo.dateOfBirth}
Giới tính: ${this.cvData.personalInfo.gender}

MỤC TIÊU NGHỀ NGHIỆP
${this.cvData.summary}

KINH NGHIỆM LÀM VIỆC
${this.cvData.experience.map(exp => `
${exp.jobTitle} tại ${exp.companyName}
${exp.startDate} - ${exp.isCurrentJob ? 'Hiện tại' : exp.endDate}
${exp.location}
${exp.description}
`).join('\n')}

HỌC VẤN
${this.cvData.education.map(edu => `
${edu.degree} - ${edu.school || edu.institution}
${edu.startDate} - ${edu.endDate}
Xếp loại: ${edu.grade}
GPA: ${edu.gpa}
`).join('\n')}

KỸ NĂNG
${this.cvData.skills.map(skill => `${skill.name}: ${skill.level}`).join('\n')}

NGOẠI NGỮ
${this.cvData.languages.map(lang => `${lang.name}: ${lang.level || lang.proficiency}`).join('\n')}

CHỨNG CHỈ
${this.cvData.certifications.map(cert => `
${cert.name} - ${cert.issuer || cert.organization}
Ngày cấp: ${cert.issueDate}
Hết hạn: ${cert.expiryDate}
`).join('\n')}

SỞ THÍCH
${this.cvData.interests}

NGƯỜI THAM CHIẾU
${this.cvData.references.map(ref => `
${ref.name} - ${ref.position}
${ref.company}
${ref.phone} - ${ref.email}
`).join('\n')}
    `;
  }

  // Image upload handler
  onImageSelect(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.cvData.personalInfo.profileImage = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  // Export dropdown toggle
  toggleExportDropdown(): void {
    this.showExportDropdown = !this.showExportDropdown;
  }

  // Preview CV
  previewCV(): void {
    // Scroll to preview panel or show in modal
    const previewElement = document.getElementById('cv-preview');
    if (previewElement) {
      previewElement.scrollIntoView({ behavior: 'smooth' });
    }
  }

  // Export CV in different formats
  exportCV(format: 'html' | 'word' | 'pdf'): void {
    this.showExportDropdown = false;
    
    if (!this.isFormValid()) {
      this.splashScreenService.showQuickFeedback('error', 'Vui lòng điền đầy đủ thông tin bắt buộc!');
      return;
    }

    this.splashScreenService.showQuickFeedback('success', `Đang tạo file ${format.toUpperCase()}...`);
    
    setTimeout(() => {
      switch (format) {
        case 'html':
          this.exportHTML();
          break;
        case 'word':
          this.exportWord();
          break;
        case 'pdf':
          this.exportPDF();
          break;
      }
      this.splashScreenService.showQuickFeedback('success', 'Xuất file thành công!');
    }, 2000);
  }

  // Export as HTML
  exportHTML(): void {
    const htmlContent = this.generateHTMLContent();
    const blob = new Blob([htmlContent], { type: 'text/html' });
    this.downloadFile(blob, 'html');
  }

  // Export as Word
  exportWord(): void {
    const wordContent = this.generateWordContent();
    const blob = new Blob([wordContent], { type: 'application/msword' });
    this.downloadFile(blob, 'doc');
  }

  // Export as PDF
  exportPDF(): void {
    const pdfContent = this.createPDFContent();
    const blob = new Blob([pdfContent], { type: 'application/pdf' });
    this.downloadFile(blob, 'pdf');
  }

  // Download file helper
  downloadFile(blob: Blob, extension: string): void {
    const element = document.createElement('a');
    const filename = `CV_${this.cvData.personalInfo.fullName.replace(/\s+/g, '_')}_${new Date().toISOString().split('T')[0]}.${extension}`;
    const url = window.URL.createObjectURL(blob);
    
    element.href = url;
    element.download = filename;
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
    window.URL.revokeObjectURL(url);
  }

  // Generate HTML content
  generateHTMLContent(): string {
    return `
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>CV - ${this.cvData.personalInfo.fullName}</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Roboto', 'Open Sans', sans-serif; line-height: 1.6; }
        .cv-container { display: flex; min-height: 100vh; }
        .cv-left-column { width: 35%; background: #f8f9fa; padding: 2rem; }
        .cv-right-column { width: 65%; padding: 2rem; }
        .profile-image { width: 120px; height: 120px; border-radius: 50%; overflow: hidden; margin: 0 auto 1rem; background: #e9ecef; }
        .profile-image img { width: 100%; height: 100%; object-fit: cover; }
        .name { font-size: 2rem; font-weight: 700; color: #ff6b35; text-align: center; margin-bottom: 0.5rem; }
        .job-title { font-size: 1.2rem; color: #666; text-align: center; margin-bottom: 2rem; }
        .section-heading { font-size: 1.3rem; font-weight: 600; color: #333; margin-bottom: 1rem; border-bottom: 2px solid #ff6b35; padding-bottom: 0.5rem; }
        .info-item { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.8rem; }
        .info-item svg { color: #ff6b35; }
        .skill-item { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem; }
        .check-icon { color: #28a745; }
        .education-item, .experience-item { margin-bottom: 1.5rem; }
        .education-item h4, .experience-item h4 { font-weight: 600; color: #333; }
        .degree, .position { font-weight: 500; color: #ff6b35; }
        .period { color: #666; font-size: 0.9rem; }
        .description, .objective-content p { margin-top: 0.5rem; color: #555; }
    </style>
</head>
<body>
    <div class="cv-container">
        <div class="cv-left-column">
            ${this.cvData.personalInfo.profileImage ? `
            <div class="profile-image">
                <img src="${this.cvData.personalInfo.profileImage}" alt="Profile">
            </div>` : ''}
            <h1 class="name">${this.cvData.personalInfo.fullName}</h1>
            <h2 class="job-title">${this.cvData.personalInfo.jobTitle}</h2>
            
            <h3 class="section-heading">Thông tin cá nhân</h3>
            ${this.cvData.personalInfo.email ? `<div class="info-item">📧 ${this.cvData.personalInfo.email}</div>` : ''}
            ${this.cvData.personalInfo.phone ? `<div class="info-item">📞 ${this.cvData.personalInfo.phone}</div>` : ''}
            ${this.cvData.personalInfo.dateOfBirth ? `<div class="info-item">📅 ${this.formatDate(this.cvData.personalInfo.dateOfBirth)}</div>` : ''}
            ${this.cvData.personalInfo.gender ? `<div class="info-item">👤 ${this.cvData.personalInfo.gender}</div>` : ''}
            
            ${this.cvData.skills.length > 0 ? `
            <h3 class="section-heading">Kỹ năng</h3>
            ${this.cvData.skills.map(skill => `<div class="skill-item">✓ ${skill.name}</div>`).join('')}` : ''}
        </div>
        
        <div class="cv-right-column">
            ${this.cvData.summary ? `
            <h3 class="section-heading">Mục tiêu nghề nghiệp</h3>
            <div class="objective-content"><p>${this.cvData.summary}</p></div>` : ''}
            
            ${this.cvData.education.length > 0 ? `
            <h3 class="section-heading">Học vấn</h3>
            ${this.cvData.education.map(edu => `
            <div class="education-item">
                <h4>${edu.school || edu.institution}</h4>
                <div class="degree">${edu.degree}</div>
                <div class="period">${this.formatMonth(edu.startDate)} - ${this.formatMonth(edu.endDate || '')}</div>
                ${edu.grade ? `<div>Xếp loại: ${edu.grade}</div>` : ''}
            </div>`).join('')}` : ''}
            
            ${this.cvData.experience.length > 0 ? `
            <h3 class="section-heading">Kinh nghiệm làm việc</h3>
            ${this.cvData.experience.map(exp => `
            <div class="experience-item">
                <h4>${exp.companyName}</h4>
                <div class="position">${exp.jobTitle}</div>
                <div class="period">${this.formatMonth(exp.startDate)} - ${exp.isCurrentJob ? 'Hiện tại' : this.formatMonth(exp.endDate || '')}</div>
                ${exp.description ? `<div class="description">${exp.description}</div>` : ''}
            </div>`).join('')}` : ''}
            
            ${this.cvData.computerSkills ? `
            <h3 class="section-heading">Tin học</h3>
            <p>${this.cvData.computerSkills}</p>` : ''}
            
            ${this.cvData.languages.length > 0 ? `
            <h3 class="section-heading">Ngoại ngữ</h3>
            ${this.cvData.languages.map(lang => `<div>${lang.name}: ${lang.level}</div>`).join('')}` : ''}
            
            ${this.cvData.interests ? `
            <h3 class="section-heading">Sở thích</h3>
            <p>${this.cvData.interests}</p>` : ''}
        </div>
    </div>
</body>
</html>`;
  }

  // Generate Word content
  generateWordContent(): string {
    return `
<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:w="urn:schemas-microsoft-com:office:word" xmlns="http://www.w3.org/TR/REC-html40">
<head>
    <meta charset="utf-8">
    <title>CV - ${this.cvData.personalInfo.fullName}</title>
    <style>
        body { font-family: Arial, sans-serif; }
        h1 { color: #ff6b35; font-size: 24pt; }
        h2 { color: #666; font-size: 14pt; }
        h3 { color: #333; font-size: 12pt; border-bottom: 2px solid #ff6b35; }
    </style>
</head>
<body>
    <h1>${this.cvData.personalInfo.fullName}</h1>
    <h2>${this.cvData.personalInfo.jobTitle}</h2>
    
    <h3>Thông tin cá nhân</h3>
    <p>Email: ${this.cvData.personalInfo.email}</p>
    <p>Điện thoại: ${this.cvData.personalInfo.phone}</p>
    
    ${this.cvData.summary ? `<h3>Mục tiêu nghề nghiệp</h3><p>${this.cvData.summary}</p>` : ''}
    
    ${this.cvData.education.length > 0 ? `
    <h3>Học vấn</h3>
    ${this.cvData.education.map(edu => `
    <p><strong>${edu.school || edu.institution}</strong><br>
    ${edu.degree}<br>
    ${this.formatMonth(edu.startDate)} - ${this.formatMonth(edu.endDate || '')}</p>`).join('')}` : ''}
    
    ${this.cvData.experience.length > 0 ? `
    <h3>Kinh nghiệm làm việc</h3>
    ${this.cvData.experience.map(exp => `
    <p><strong>${exp.companyName}</strong><br>
    ${exp.jobTitle}<br>
    ${this.formatMonth(exp.startDate)} - ${exp.isCurrentJob ? 'Hiện tại' : this.formatMonth(exp.endDate || '')}<br>
    ${exp.description}</p>`).join('')}` : ''}
</body>
</html>`;
  }

  // Date formatting helpers
  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
  }

  formatMonth(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return `${date.getMonth() + 1}/${date.getFullYear()}`;
  }

  goBack(): void {
    this.location.back();
  }

  ngOnDestroy(): void {
    // Clear editing CV when leaving component
    this.candidateService.clearCurrentEditingCV();
  }

  // Update methods for contenteditable elements
  updateFullName(event: Event): void {
    const target = event.target as HTMLElement;
    this.cvData.personalInfo.fullName = target.textContent || '';
  }

  updateJobTitle(event: Event): void {
    const target = event.target as HTMLElement;
    this.cvData.personalInfo.jobTitle = target.textContent || '';
  }

  updateSkillName(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.skills[index].name = target.textContent || '';
  }

  updateSummary(event: Event): void {
    const target = event.target as HTMLElement;
    this.cvData.summary = target.textContent || '';
  }

  updateEducationSchool(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.education[index].school = target.textContent || '';
  }

  updateEducationDegree(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.education[index].degree = target.textContent || '';
  }

  updateEducationGrade(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.education[index].grade = target.textContent || '';
  }

  updateExperienceCompany(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.experience[index].companyName = target.textContent || '';
  }

  updateExperienceJobTitle(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.experience[index].jobTitle = target.textContent || '';
  }

  updateExperienceDescription(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.experience[index].description = target.textContent || '';
  }

  updateComputerSkills(event: Event): void {
    const target = event.target as HTMLElement;
    this.cvData.computerSkills = target.textContent || '';
  }

  updateLanguageName(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.languages[index].name = target.textContent || '';
  }

  updateLanguageLevel(event: Event, index: number): void {
    const target = event.target as HTMLElement;
    this.cvData.languages[index].level = target.textContent || '';
  }

  updateInterests(event: Event): void {
    const target = event.target as HTMLElement;
    this.cvData.interests = target.textContent || '';
  }

  // Save CV method
  saveCV(): void {
    if (!this.isFormValid()) {
      this.splashScreenService.showQuickFeedback('error', 'Vui lòng điền đầy đủ thông tin bắt buộc!');
      return;
    }

    // Update timestamp
    this.cvData.updatedDate = new Date().toISOString().split('T')[0];

    if (this.isEditMode) {
      // Update existing CV
      this.candidateService.updateCV(this.cvId!, this.cvData).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.splashScreenService.showQuickFeedback('success', 'CV đã được cập nhật thành công!');
            this.router.navigate(['/website/cv-management']);
          } else {
            this.splashScreenService.showQuickFeedback('error', 'Lỗi khi cập nhật CV!');
          }
        },
        error: (error) => {
          console.error('Error updating CV:', error);
          this.splashScreenService.showQuickFeedback('error', 'Lỗi khi cập nhật CV!');
        }
      });
    } else {
      // Create new CV
      this.candidateService.createCV(this.cvData).subscribe({
        next: (response) => {
          if (response.isSuccess) {
            this.splashScreenService.showQuickFeedback('success', 'CV đã được tạo thành công!');
            this.router.navigate(['/website/cv-management']);
          } else {
            this.splashScreenService.showQuickFeedback('error', 'Lỗi khi tạo CV!');
          }
        },
        error: (error) => {
          console.error('Error creating CV:', error);
          this.splashScreenService.showQuickFeedback('error', 'Lỗi khi tạo CV!');
        }
      });
    }
  }
}