<template>
    <div>
        <b-container>
            <b-row class="mb-4 text-center">
                <div>
                    <a href="https://mars.nasa.gov/msl/surface-experience/?drive=2176&site=82" target="_blank">
                        <img src="../assets/Nasa_logo.png" />
                    </a>
                </div>
            </b-row>
            <b-row>
                <h1 class="mb-4"><strong>Mars Rover Instruction Submission</strong></h1>
                <b-form @submit="onSubmit" v-if="showForm">
                    <b-form-group id="grid-bounds-group" label-for="grid-bounds">
                        <template #label>
                            <div class="m-0">
                                <span>Grid Bounds</span>
                                <span v-b-tooltip.hover.right :title="boundsTooltip"><font-awesome-icon icon="question-circle" /></span>
                            </div>
                        </template>
                        <b-form-input id="grid-bounds" v-model="form.gridBounds" type="text" placeholder="Enter grid bounds" required :trim="true" :disabled="gridBoundsSubmitted" />
                        <b-form-invalid-feedback :state="gridBoundsValid">
                            Grid Bounds must be a two digit value (e.x. 55).
                        </b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group id="position-heading-group" label-for="position-heading">
                        <template #label>
                            <div class="m-0">
                                <span>Position and Heading</span>
                                <span v-b-tooltip.hover.right :title="posHeadingTooltip"><font-awesome-icon icon="question-circle" /></span>
                            </div>
                        </template>
                        <b-form-input id="position-heading" v-model="form.positionHeading.toUpperCase()" type="text" placeholder="Enter rover position and heading" required :trim="true" />
                        <b-form-invalid-feedback :state="positionHeadingValid">
                            Position and Heading must be 2 digits followed by a cardinal compass point (e.x. 12N).
                        </b-form-invalid-feedback>
                    </b-form-group>

                    <b-form-group id="movement-group" label-for="movement">
                        <template #label>
                            <div class="m-0">
                                <span>Requested Movement</span>
                                <span v-b-tooltip.hover.right :title="movementTooltip"><font-awesome-icon icon="question-circle" /></span>
                            </div>
                        </template>
                        <b-form-input id="movement" v-model="form.movement" type="text" placeholder="Enter requested movement for rover" required :trim="true" />
                        <b-form-invalid-feedback :state="movementValid">
                            Movement instructions must contain only turn/move values (e.x. MMRMMRMRRM).
                        </b-form-invalid-feedback>
                    </b-form-group>

                    <b-button class="mt-4" type="submit" :disabled="actionLoading">
                        <b-spinner v-if="actionLoading" class="spinner" small type="grow"></b-spinner>
                        <span>{{actionLoading ? 'SUBMITTING...' : 'SUBMIT'}}</span>
                    </b-button>
                </b-form>
            </b-row>

            <b-row class="help-guide">
                <div>
                    <b-card header-tag="header">
                        <template #header>
                            <h5 class="mb-0 text-center">Directions</h5>
                        </template>
                        <b-card-text>
                            <div class="sub-group mb-3">
                                <p>1. Input two digit string to establish grid bounds (e.x. 55)</p>
                                <p>2. Input two digit string followed by a cardinal compass point to confirm current position and heading for rover (e.x. 12N) </p>
                                <p>3. Input a movement string that only contains turn/move values (e.x. MMRMMRMRRM).</p>
                            </div>
                        </b-card-text>
                    </b-card>
                </div>
            </b-row>

        </b-container>

        <b-alert :show="submissionSuccessMessage !== ''" class="position-fixed fixed-bottom m-0 rounded-0 text-center" fade variant="success">{{submissionSuccessMessage}}</b-alert>
        <b-alert :show="submissionFailMessage !== ''" class="position-fixed fixed-bottom m-0 rounded-0 text-center" fade variant="danger">{{submissionFailMessage}}</b-alert>
    </div>
</template>

<script>
    import roverInstruction from '../classes/roverInstruction';
    import axios from 'axios';

    export default {
        name: 'MarsRover',
        data() {
            return {
                showForm: true,
                submissionSuccessMessage: '',
                submissionFailMessage: '',
                http: null,
                actionLoading: false,
                gridBoundsSubmitted: false,
                gridBoundsValid: true,
                positionHeadingValid: true,
                movementValid: true,
                form: new roverInstruction(),
                boundsTooltip: 'Represents the bounds of the grid atop the plateau. Bounds are represented by XY pair of numbers that correspond with the northeast corner of the plateau.\
                                Once bounds are confirmed this field will become disabled.',
                posHeadingTooltip: 'Represents the current position and heading of the rover. This is represented by an XYZ string.',
                movementTooltip: 'Represents the desired movement for the rover indicated by a string combination of L, R, and M. L and R instruct the rover to rotate left or right 90 degrees,\
                                 respectively, whereas M instructs the rover to move forward one grid point.'
            }
        },
        mounted: function () {
            this.http = axios.create({
                baseURL: 'https://localhost:44330/api/'
            });
        },
        methods: {
            async onSubmit(event) {
                event.preventDefault();

                if (!this.validateFields()) {
                    return;
                }

                this.actionLoading = true;

                try {
                    let response = await this.http.post('marsRover', this.form, { responseType: "application/json" });
                    this.resetForm();
                    this.actionLoading = false;

                    this.submissionSuccessMessage = response.data;
                    this.gridBoundsSubmitted = true;
                } catch (e) {
                    this.actionLoading = false;
                    this.submissionFailMessage = e.response.data;
                }

                setTimeout(() => {
                    this.submissionSuccessMessage = '';
                    this.submissionFailMessage = '';
                }, 8000);
            },
            resetForm() {
                this.form.positionHeading = '';
                this.form.movement = '';

                this.showForm = false;
                this.$nextTick(() => {
                    this.showForm = true;
                })
            },
            validateFields() {
                //grid bounds validation
                const boundsRegex = new RegExp('^[0-9]{2}$');
                this.gridBoundsValid = boundsRegex.test(this.form.gridBounds);

                //position and heading validation
                const posHeadingRegex = new RegExp('^[0-9]{2}[NnEeSsWw]{1}$');
                this.positionHeadingValid = posHeadingRegex.test(this.form.positionHeading);

                //movement validation
                const movementRegex = new RegExp('^[L,l,R,r,M,m]+$');
                this.movementValid = movementRegex.test(this.form.movement);

                return this.gridBoundsValid && this.positionHeadingValid && this.movementValid;
            }
        }
    }
</script>

<style scoped>
    .container {
        max-width: 720px;
    }

    h1 {
        font-size: 38px;
    }

    ::v-deep label {
        font-size: 16px;
        margin-bottom: 3px;
    }

    form div {
        margin-bottom: 15px;
    }

    form input {
        background: var(--white);
        border-color: var(--blue);
        height: 36px;
        font-size: 14px;
        font-family: Arial, sans-serif;
    }

    form textarea {
        background: var(--white);
        border-color: var(--blue);
        font-size: 16px;
        font-family: Arial, sans-serif;
    }

    button {
        height: 36px;
        font-size: 16px;
        width: 100%;
        background: var(--blue);
        color: var(--white);
    }

    .spinner {
        margin-right: 10px;
    }

    .btn:hover {
        background-color: #1a5dadc9;
    }

    .btn-secondary:disabled, .btn-secondary.disabled {
        background: var(--blue);
    }

    ::v-deep .form-control {
        color: var(--text) !important;
    }

    .container img {
        width: 200px;
    }

    .help-guide {
        margin-top: 80px;
    }

    .sub-group p {
        margin: 0px;
        margin-left: 20px;
    }

    form span {
        margin-right: 10px;
    }

    .fa-question-circle {
        color: #1a5dad;
    }
</style>
