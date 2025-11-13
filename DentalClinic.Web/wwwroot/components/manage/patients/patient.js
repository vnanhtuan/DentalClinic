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
                address: '',
                city: '',
                emergencyName: '',
                emergencyPhone: '',
                notes: '',

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
                { value: 2, title: 'Địa chỉ & Liên hệ khẩn cấp', icon: 'mdi-map-marker-account', isComplete: false, requiredFields: ['address'] },
                { value: 3, title: 'Lịch sử Y tế', icon: 'mdi-medical-bag', isComplete: false, requiredFields: ['bloodType'] },
                { value: 4, title: 'Bảo hiểm & Hoàn tất', icon: 'mdi-file-check', isComplete: false, requiredFields: ['insuranceProvider'] },
            ],
            // Dữ liệu cho Dropdown/Select
            bloodTypes: ['A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-', 'Không rõ'],
            allergyOptions: ['Thuốc Penicillin', 'Latex', 'Thức ăn (Hải sản)', 'Khác'],
            occupationOptions: ['Bác sĩ', 'Kỹ sư', 'Học sinh/Sinh viên', 'Nghề nghiệp khác'],
            referralSources: ['Website', 'Google Search', 'Bạn bè', 'Truyền thông', 'Khác'],
            
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
        isCurrentStepValid() {
            // Kiểm tra xem tất cả các fields bắt buộc của step hiện tại đã được điền chưa
            const currentStepData = this.steps.find(s => s.value === this.activeStep);
            if (!currentStepData) return false;

            for (const field of currentStepData.requiredFields) {
                const value = this.patient[field];
                // Kiểm tra null, undefined, chuỗi rỗng, hoặc mảng rỗng (cho multi-select)
                if (value === null || value === undefined || value === '' || (Array.isArray(value) && value.length === 0)) {
                    return false;
                }
            }
            return true;
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
        closeForm() {
            this.$emit('update:show', false);
        },
        resetForm() {
            this.patient = Object.assign({}, this.defaultPatient);
            this.$refs.patientForm?.resetValidation();
            this.error = null;
        },async validateAndMarkComplete() {
            // Sử dụng ref của form hiện tại để chạy validation chính xác của Vuetify
            const formRef = this.$refs['formStep' + this.activeStep];
            if (formRef) {
                const { valid } = await formRef.validate();
                return valid;
            }
            // Nếu không có ref, chỉ kiểm tra các field bắt buộc
            return this.isCurrentStepValid;
        },

        async nextStep() {
            if (this.loading) return;
            
            // 1. Chạy validation chính xác của Vuetify
            const isValid = await this.validateAndMarkComplete();

            if (isValid) {
                // 2. Đánh dấu bước hiện tại là hoàn thành
                this.steps[this.activeStep - 1].isComplete = true;

                // 3. Chuyển sang bước tiếp theo
                if (this.activeStep < this.steps.length) {
                    this.activeStep++;
                }
            } else {
                this.showWarningDialog('Vui lòng điền đầy đủ các thông tin bắt buộc trong bước này.', 'Thông tin chưa hợp lệ');
            }
        },

        prevStep() {
            if (this.activeStep > 1) {
                this.activeStep--;
            }
        },

        goToStep(stepValue) {
            // Nếu click vào bước trước đó hoặc bước hiện tại đã hoàn thành
            if (stepValue < this.activeStep || this.steps[stepValue - 1].isComplete) {
                this.activeStep = stepValue;
            }
            // Không cho phép nhảy qua bước chưa hoàn thành
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