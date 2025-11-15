import { patientApi } from './patient-api.js';
import { MessageDialogMixin } from '../../../js/manage/mixins/messageDialogMixin.js';
import { PAGE_SIZE_OPTIONS, DEFAULT_PAGE_SIZE } from '../../../js/manage/constants/paginationConstants.js';
import { PaginationComponent } from '../../../js/manage/components/paginationComp.js';

const patientFormResponse = await fetch('/components/manage/patients/patientForm.html');
const patientFormHtml = await patientFormResponse.text();

export const PatientFormComponent = {
    template: patientFormHtml,
    props: {
        show: {
            type: Boolean,
            required: true
        }
    },
    emits: ['update:show', 'patient-created'],
    data() {
        return {
            loading: false,
            loading: false,
            activeStep: 1,
            // Mô hình dữ liệu bệnh nhân (tổng hợp từ 4 bước)
            patient: {
                // Step 1: Basic Info
                fullName: '',
                phone: '',
                email: '',
                dateOfBirth: null,
                gender: null, // M, F, O

                // Step 2: Address & Contact
                national: null,
                address: '',
                otherAddress: '',
                city: '',
                district: '',
                emergencyName: '',
                emergencyPhone: '',
                notes: '',

                // Step 3: Needs
                pathologies: [], // Multi-select
                intendedTreatments: [], // Multi-select
                estimatedCost: null,

                // Step 3: Medical History
                bloodType: null,
                allergies: [], // Multi-select
                occupation: null,
                isSmoker: false,
                isOnMedication: false,

                // Step 4: Insurance & Final
                insuranceProvider: '',
                policyNumber: '',
                referralSource: null,
                finalNotes: '',
            },
            // Metadata cho Stepper
            steps: [
                { value: 1, title: 'Thông tin cơ bản', icon: 'mdi-account-details', isComplete: false, requiredFields: ['fullName', 'phone'] },
                { value: 2, title: 'Liên hệ', icon: 'mdi-map-marker-account', isComplete: false, requiredFields: ['address'] },
                { value: 3, title: 'Nhu cầu', icon: 'mdi-map-marker-account', isComplete: false, requiredFields: ['pathology'] },
                { value: 4, title: 'Tiểu sử bệnh', icon: 'mdi-medical-bag', isComplete: false, requiredFields: ['bloodType'] },
                { value: 5, title: 'Lịch hẹn', icon: 'mdi-file-check', isComplete: false, requiredFields: ['date'] },
                { value: 6, title: 'Relative', icon: 'mdi-file-check', isComplete: false, requiredFields: [] },
                { value: 7, title: 'Marketing', icon: 'mdi-file-check', isComplete: false, requiredFields: ['source'] },
            ],
            // Dữ liệu cho Dropdown/Select
            bloodTypes: ['A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-', 'Không rõ'],
            pathologyOptions: ['Vôi răng', 'Khớp cắn ngược', 'Viêm lợi, nhiều cao răng', 'Tái khám', 'Răng hô','Răng thưa'],
            intendOptions: ['Chỉnh nha', 'Cấy ghép implant', 'Tẩy trắng răng', 'Phục hình răng sứ', 'Điều trị tủy', 'Nhổ răng khôn'],
            allergyOptions: ['Thuốc Penicillin', 'Latex', 'Thức ăn (Hải sản)', 'Khác'],
            occupationOptions: ['Bác sĩ', 'Kỹ sư', 'Học sinh/Sinh viên', 'Nghề nghiệp khác'],
            referralSources: ['Website', 'Google Search', 'Bạn bè', 'Truyền thông', 'Khác'],
            nationals: ['Việt Nam', 'Hoa Kỳ', 'Canada', 'Úc', 'Anh', 'Pháp', 'Đức', 'Nhật Bản', 'Hàn Quốc', 'Khác'],
            
            // Validation Rules
            rules: {
                required: v => !!v || 'Thông tin bắt buộc.',
                phone: v => {
                    if (!v) return true;
                    return /^\d{10,11}$/.test(v) || 'Số điện thoại không hợp lệ.';
                },
                email: v => {
                    if (!v) return true;
                    return /.+@.+\..+/.test(v) || 'Email không hợp lệ.';
                }
            },
        };
    },
    computed: {
        isMobile() {
            // Dùng $vuetify.display.smAndDown để bắt responsive
            return this.$vuetify.display.smAndDown;
        },
        isSaveDisabled() {
            // Lấy ra tất cả các trường bắt buộc từ tất cả các bước
            const allRequiredFields = this.steps.flatMap(step => step.requiredFields);

            for (const field of allRequiredFields) {
                const value = this.patient[field];
                // Kiểm tra null, undefined, chuỗi rỗng, hoặc mảng rỗng (cho multi-select)
                if (value === null || value === undefined || value === '' || (Array.isArray(value) && value.length === 0)) {
                    return true; // Nếu thiếu 1 trường, vô hiệu hóa nút Lưu
                }
            }
            return false; // Tất cả đã được điền
        },
    },
    watch: {
        show(newVal) {
            if (newVal) {
                this.activeStep = 1;
                this.resetForm();
            }
        }
    },
    methods: {
        isStepCompleted(requiredFields) {
            for (const field of requiredFields) {
                const value = this.patient[field];
                if (value === null || value === undefined || value === '' || (Array.isArray(value) && value.length === 0)) {
                    return false;
                }
            }
            return true;
        },
        closeForm() {
            this.$emit('update:show', false);
        },
        resetForm() {
            this.patient = Object.assign({}, this.defaultPatient);
            this.$refs.patientForm?.resetValidation();
            this.error = null;
        },
        async savePatient() {
            const { valid } = await this.$refs.patientForm.validate();
            if (!valid) {
                this.showWarningDialog('Vui lòng điền đầy đủ thông tin bắt buộc.', 'Thông tin chưa hợp lệ');
                return;
            }

            this.loading = true;
            this.error = null;

            try {
                const { valid } = await this.$refs.patientForm.validate();
                if (!valid) {
                    this.showWarningDialog('Vui lòng kiểm tra lại các trường có lỗi (màu đỏ) trước khi lưu.', 'Thông tin chưa hợp lệ');
                    this.loading = false;
                    return;
                }

                const createDto = {
                    fullName: this.patient.fullName,
                    phone: this.patient.phone,
                    email: this.patient.email || null,
                    dateOfBirth: this.patient.dateOfBirth,
                };
                
                const newPatient = await patientApi.create(createDto); 
                
                this.showSuccessDialog(
                    `Đã thêm khách hàng **${newPatient.fullName}** thành công.`,
                    'Thêm thành công',
                    () => {
                        this.closeForm(); // Đóng form
                        this.$emit('patient-created', newPatient.patientId); // Báo cho cha biết
                    }
                );
            } catch (err) {
                const errorMessage = err.response?.data?.message || 'Đã xảy ra lỗi khi lưu thông tin khách hàng.';
                this.showErrorDialog(errorMessage, 'Lỗi lưu dữ liệu');
            } finally {
                this.loading = false;
            }
        }
    }
}