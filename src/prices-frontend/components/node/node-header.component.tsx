import { Chip, Stack, Typography } from "@mui/material";
import { memo } from "react";
import { PricingNode } from "../../gql/codegen/graphql";

type NodeHeaderProps = {
    pricingNode: PricingNode;
};

export const NodeHeader = memo<NodeHeaderProps>(function NodeHeader(props: NodeHeaderProps) {
    const { pricingNode } = props;

    return (
        <Stack direction="row" alignItems="center" gap={2} mb={4}>
            <Stack direction="row" alignItems="center" gap={2} marginLeft={2} marginRight="auto">
                <Typography
                    variant="h2"
                    fontWeight={600}
                    sx={(theme) => ({
                        fontFamily: theme.typography["fontFamilyAlt"],
                    })}
                >
                    {pricingNode.name}
                </Typography>
                <Chip
                    label={pricingNode.regionalTransmissionOperatorId}
                    size="small"
                    sx={(theme) => ({
                        borderRadius: theme.spacing(2),
                    })}
                />
            </Stack>
            <Stack direction="row"></Stack>
        </Stack>
    );
});
