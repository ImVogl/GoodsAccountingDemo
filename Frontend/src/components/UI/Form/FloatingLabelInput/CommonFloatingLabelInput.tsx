import classes from './CommonFloatingLabelInput.module.css'
import { FC, ReactElement } from 'react';

export interface IInputProps extends React.InputHTMLAttributes<HTMLInputElement>{
    label: string;
    error?: string;
}

/// Компонента с общестпользуемой кнопкой.
export const CommonFloatingLabelInput: FC<IInputProps> = ({ label, error, ...props }): ReactElement => {
    const valid = !(error === null || error === '' || error === undefined);
    return (
        <div>
            <label htmlFor={props.id} className={classes.commonLabel}>{label}</label>
            <input {...props} className={valid ? classes.commonInput : classes.error} placeholder={label} />
            <small>{error}</small>
        </div>
    );
}